﻿/*
 * Copyright 2018 James Courtney
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace FlatSharp.TypeModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using FlatSharp.Attributes;

    /// <summary>
    /// Describes the schema of a FlatBuffer table. Tables, analgous to classes, provide for mutable schema definitions over time
    /// by declaring a vatble mapping of field indexes to offsets.
    /// </summary>
    public class TableTypeModel : RuntimeTypeModel
    {
        private const int FileIdentifierSize = 4;

        private readonly string ParseClassName = "tableReader_" + Guid.NewGuid().ToString("n");

        /// <summary>
        /// Maps vtable index -> type model.
        /// </summary>
        private readonly Dictionary<int, TableMemberModel> memberTypes = new Dictionary<int, TableMemberModel>();

        /// <summary>
        /// Contains the vtable indices that have already been occupied.
        /// </summary>
        private readonly HashSet<int> occupiedVtableSlots = new HashSet<int>();
        private ConstructorInfo? preferredConstructor;

        internal TableTypeModel(Type clrType, TypeModelContainer typeModelProvider) : base(clrType, typeModelProvider)
        {
        }

        /// <summary>
        /// Gets the schema type.
        /// </summary>
        public override FlatBufferSchemaType SchemaType => FlatBufferSchemaType.Table;

        /// <summary>
        /// Layout when in a vtable.
        /// </summary>
        public override ImmutableArray<PhysicalLayoutElement> PhysicalLayout => new PhysicalLayoutElement[] { new PhysicalLayoutElement(sizeof(uint), sizeof(uint)) }.ToImmutableArray();

        /// <summary>
        /// Tables can have vectors and other arbitrary data.
        /// </summary>
        public override bool IsFixedSize => false;

        /// <summary>
        /// Tables can't be part of structs.
        /// </summary>
        public override bool IsValidStructMember => false;

        /// <summary>
        /// Tables can be part of tables.
        /// </summary>
        public override bool IsValidTableMember => true;

        /// <summary>
        /// Tables can be part of unions.
        /// </summary>
        public override bool IsValidUnionMember => true;

        /// <summary>
        /// Tables can be part of vectors.
        /// </summary>
        public override bool IsValidVectorMember => true;

        /// <summary>
        /// Tables can't be keys of sorted vectors.
        /// </summary>
        public override bool IsValidSortedVectorKey => false;

        /// <summary>
        /// Tables are written by reference.
        /// </summary>
        public override bool SerializesInline => false;

        /// <summary>
        /// Gets the maximum used index in this vtable.
        /// </summary>
        public int MaxIndex => this.occupiedVtableSlots.Max();

        /// <summary>
        /// Maps the table index to the details about that member.
        /// </summary>
        public IReadOnlyDictionary<int, TableMemberModel> IndexToMemberMap => this.memberTypes;

        /// <summary>
        /// The property type used as a key.
        /// </summary>
        public TableMemberModel? KeyMember { get; private set; }

        /// <summary>
        /// Gets the maximum size of a table assuming all members are populated include the vtable offset. 
        /// Does not consider alignment of the table, but does consider worst-case alignment of the members.
        /// </summary>
        internal int NonPaddedMaxTableInlineSize
        {
            // add sizeof(int) for soffset_t to vtable.
            get => this.IndexToMemberMap.Values.Sum(x => x.ItemTypeModel.MaxInlineSize) + sizeof(int);
        }

        public override ConstructorInfo? PreferredSubclassConstructor => this.preferredConstructor;

        public override void Initialize()
        {
            var tableAttribute = this.ClrType.GetCustomAttribute<FlatBufferTableAttribute>();
            if (tableAttribute == null)
            {
                throw new InvalidFlatBufferDefinitionException($"Can't create table type model from type {this.ClrType.Name} because it does not have a [FlatBufferTable] attribute.");
            }

            ValidateFileIdentifier(tableAttribute.FileIdentifier);

            EnsureClassCanBeInheritedByOutsideAssembly(this.ClrType, out this.preferredConstructor);

            var properties = this.ClrType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(x => new
                {
                    Property = x,
                    Attribute = x.GetCustomAttribute<FlatBufferItemAttribute>(),
                })
                .Where(x => x.Attribute is not null)
                .Select(x => new
                {
                    x.Property,
                    Attribute = x.Attribute!, // not null by virtue of filter above.
                    ItemTypeModel = this.typeModelContainer.CreateTypeModel(x.Property.PropertyType),
                })
                .ToList();

            ushort maxIndex = 0;

            if (!properties.Any())
            {
                throw new InvalidFlatBufferDefinitionException($"Can't create table type model from type {this.GetCompilableTypeName()} because it does not have any non-static, non-deprecated [FlatBufferItem] properties.");
            }

            foreach (var property in properties)
            {
                ushort index = property.Attribute.Index;
                maxIndex = Math.Max(index, maxIndex);

                TableMemberModel model = new TableMemberModel(
                    property.ItemTypeModel,
                    property.Property,
                    property.Attribute);

                property.ItemTypeModel.AdjustTableMember(model);

                if (property.Attribute.Key)
                {
                    if (this.KeyMember != null)
                    {
                        throw new InvalidFlatBufferDefinitionException($"Table {this.GetCompilableTypeName()} has more than one [FlatBufferItemAttribute] with Key set to true.");
                    }

                    if (!property.ItemTypeModel.IsValidSortedVectorKey)
                    {
                        throw new InvalidFlatBufferDefinitionException($"Table {this.GetCompilableTypeName()} declares a key property on a type that that does not support being a key in a sorted vector.");
                    }

                    if (!property.ItemTypeModel.TryGetSpanComparerType(out _))
                    {
                        throw new InvalidFlatBufferDefinitionException($"Table {this.GetCompilableTypeName()} declares a key property on a type whose type model does not supply a ISpanComparer type.");
                    }

                    if (model.IsDeprecated)
                    {
                        throw new InvalidFlatBufferDefinitionException($"Table {this.GetCompilableTypeName()} declares a key property that is deprecated.");
                    }

                    this.KeyMember = model;
                }

                ValidateSortedVector(model);
                this.ValidateForceWrite(model);

                for (int i = 0; i < model.ItemTypeModel.PhysicalLayout.Length; ++i)
                {
                    if (!this.occupiedVtableSlots.Add(index + i))
                    {
                        throw new InvalidFlatBufferDefinitionException($"FlatBuffer Table {this.GetCompilableTypeName()} already defines a property with index {index}. This may happen when unions are declared as these are double-wide members.");
                    }
                }

                this.memberTypes[index] = model;
            }
        }

        private void ValidateForceWrite(TableMemberModel model)
        {
            if (model.ForceWrite)
            {
                if (!model.ItemTypeModel.ClassifyContextually(this.SchemaType).IsRequiredValue())
                {
                    throw new InvalidFlatBufferDefinitionException($"Property '{model.PropertyInfo.Name}' on table '{this.GetCompilableTypeName()}' declares the {nameof(FlatBufferItemAttribute.ForceWrite)} option, but the type is not supported for force write.");
                }
            }
        }

        private static void ValidateSortedVector(TableMemberModel model)
        {
            if (model.IsSortedVector)
            {
                if (model.ItemTypeModel.SchemaType != FlatBufferSchemaType.Vector)
                {
                    throw new InvalidFlatBufferDefinitionException($"Property '{model.PropertyInfo.Name}' declares the sortedVector option, but the underlying type was not a vector.");
                }

                if (!model.ItemTypeModel.TryGetUnderlyingVectorType(out ITypeModel? memberTypeModel))
                {
                    throw new InvalidFlatBufferDefinitionException($"Property '{model.PropertyInfo.Name}' declares the sortedVector option, but the underlying type model did not report the underlying vector type.");
                }

                if (memberTypeModel.SchemaType != FlatBufferSchemaType.Table)
                {
                    throw new InvalidFlatBufferDefinitionException($"Property '{model.PropertyInfo.Name}' declares a sorted vector, but the member is not a table. Type = {model.ItemTypeModel.GetCompilableTypeName()}.");
                }

                if (!memberTypeModel.TryGetTableKeyMember(out TableMemberModel? member))
                {
                    throw new InvalidFlatBufferDefinitionException($"Property '{model.PropertyInfo.Name}' declares a sorted vector, but the member does not have a key defined. Type = {model.ItemTypeModel.GetCompilableTypeName()}.");
                }

                if (!member.ItemTypeModel.TryGetSpanComparerType(out _))
                {
                    throw new InvalidFlatBufferDefinitionException($"Property '{model.PropertyInfo.Name}' declares a sorted vector, but the key does not have an implementation of ISpanComparer. Keys must be non-nullable scalars or strings. KeyType = {model.ItemTypeModel.GetCompilableTypeName()}");
                }

                if (member.ItemTypeModel.PhysicalLayout.Length != 1)
                {
                    throw new InvalidFlatBufferDefinitionException($"Property '{model.PropertyInfo.Name}' declares a sorted vector, but the sort key's vtable is not compatible with sorting. KeyType = {model.ItemTypeModel.GetCompilableTypeName()}");
                }
            }
        }

        internal static void EnsureClassCanBeInheritedByOutsideAssembly(Type type, out ConstructorInfo defaultConstructor)
        {
            string typeName = CSharpHelpers.GetCompilableTypeName(type);

            if (!type.IsClass)
            {
                throw new InvalidFlatBufferDefinitionException($"Can't create type model from type {typeName} because it is not a class.");
            }
            
            if (type.IsSealed)
            {
                throw new InvalidFlatBufferDefinitionException($"Can't create type model from type {typeName} because it is sealed.");
            }

            if (type.IsAbstract)
            {
                throw new InvalidFlatBufferDefinitionException($"Can't create type model from type {typeName} because it is abstract.");
            }

            if (type.BaseType != typeof(object))
            {
                throw new InvalidFlatBufferDefinitionException($"Can't create type model from type {typeName} its base class is not System.Object.");
            }

            if (!type.IsPublic && !type.IsNestedPublic)
            {
                throw new InvalidFlatBufferDefinitionException($"Can't create type model from type {typeName} because it is not public.");
            }

            var defaultCtor =
                type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(c => c.GetParameters().Length == 0)
                .SingleOrDefault();

            var specialCtor = 
                type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(c => c.GetParameters().Length == 1)
                .Where(c => c.GetParameters()[0].ParameterType == typeof(FlatBufferDeserializationContext))
                .SingleOrDefault();

            static bool IsVisible(ConstructorInfo c) => c.IsPublic || c.IsFamily || c.IsFamilyOrAssembly;

            if (specialCtor is not null)
            {
                if (!IsVisible(specialCtor))
                {
                    throw new InvalidFlatBufferDefinitionException($"Constructor for '{typeName}' accepting {nameof(FlatBufferDeserializationContext)} is not visible to subclasses outside the assembly.");
                }

                defaultConstructor = specialCtor;
            }
            else if (defaultCtor is not null)
            {
                if (!IsVisible(defaultCtor))
                {
                    throw new InvalidFlatBufferDefinitionException($"Default constructor for '{typeName}' is not visible to subclasses outside the assembly.");
                }

                defaultConstructor = defaultCtor;
            }
            else
            {
                throw new InvalidFlatBufferDefinitionException($"Unable to find a usable constructor for '{typeName}'. The type must supply a default constructor or single parameter constructor accepting '{nameof(FlatBufferDeserializationContext)}' that is visible to subclasses outside the assembly.");
            }
        }

        private static void ValidateFileIdentifier(string? fileIdentifier)
        {
            if (!string.IsNullOrEmpty(fileIdentifier))
            {
                if (fileIdentifier.Length != FileIdentifierSize)
                {
                    throw new InvalidFlatBufferDefinitionException($"File identifier '{fileIdentifier}' is invalid. FileIdentifiers must be exactly {FileIdentifierSize} ASCII characters.");
                }

                for (int i = 0; i < fileIdentifier.Length; ++i)
                {
                    char c = fileIdentifier[i];
                    if (c >= 128)
                    {
                        throw new InvalidFlatBufferDefinitionException($"File identifier '{fileIdentifier}' contains non-ASCII characters. Character '{c}' is invalid.");
                    }
                }
            }
        }

        public override CodeGeneratedMethod CreateSerializeMethodBody(SerializationCodeGenContext context)
        {
            var type = this.ClrType;
            int maxIndex = this.MaxIndex;
            int maxInlineSize = this.NonPaddedMaxTableInlineSize;

            // Start by asking for the worst-case number of bytes from the serializationcontext.
            string methodStart =
$@"
                int tableStart = {context.SerializationContextVariableName}.{nameof(SerializationContext.AllocateSpace)}({maxInlineSize}, sizeof(int));
                {context.SpanWriterVariableName}.{nameof(SpanWriterExtensions.WriteUOffset)}({context.SpanVariableName}, {context.OffsetVariableName}, tableStart, {context.SerializationContextVariableName});
                int currentOffset = tableStart + sizeof(int); // skip past vtable soffset_t.

                int vtableLength = 4;
                Span<byte> vtable = stackalloc byte[{4 + 2 * (maxIndex + 1)}];
";

            List<string> getters = new();
            List<string> prepareBlocks = new();
            List<string> writeBlocks = new();

            List<(
                int vtableIndex, 
                int modelIndex, 
                string valueVariableName,
                PhysicalLayoutElement layout, 
                TableMemberModel model)> items = new();

            foreach (var item in this.IndexToMemberMap)
            {
                var index = item.Key;
                var memberModel = item.Value;

                if (memberModel.IsDeprecated)
                {
                    continue;
                }

                string valueName = $"index{index}Value";
                string getter = memberModel.PropertyInfo.Name;
                if (!string.IsNullOrEmpty(memberModel.CustomGetter))
                {
                    getter = memberModel.CustomGetter;
                }

                getters.Add($"var {valueName} = {context.ValueVariableName}.{getter};");
                
                for (int i = 0; i < memberModel.ItemTypeModel.PhysicalLayout.Length; ++i)
                {
                    items.Add((index, i, valueName, memberModel.ItemTypeModel.PhysicalLayout[i], memberModel));
                }
            }

            // Pack according to the following:
            // Rule 1: Bigger alignments come first. 
            //   The biggest (practical) alignment is 8 bytes. It 
            //   is beneficial to put these first because tables are 
            //   guaranteed to be 4-byte aligned, so we have a 50% 
            //   chance of 0 padding and a 50% chance of 4 bytes of 
            //   padding, which isn't bad.
            // Rule 2: Within an alignment group, prefer smaller sized items.
            //   We are attempting to minimize padding within an alignment group.
            //   8-byte aligned structs will often not truncate on an 8 bye boundary,
            //   so the next item will need padding to align. By moving these to
            //   the end, we can hopefully minimize internal padding within an
            //   alignment group.
            items = items
                .OrderByDescending(x => x.layout.Alignment)
                .ThenBy(x => x.layout.InlineSize)
                .ThenByDescending(x => x.vtableIndex)
                .ToList();

            foreach (var t in items)
            {
                prepareBlocks.Add(this.GetPrepareSerializeBlock(
                    t.vtableIndex, 
                    t.modelIndex, 
                    t.valueVariableName, 
                    t.layout, 
                    t.model, 
                    context));

                if (t.modelIndex == 0 && !t.model.ItemTypeModel.SerializesInline)
                {
                    writeBlocks.Add($@"
                    if ({OffsetVariableName(t.vtableIndex, 0)} != tableStart)
                    {{
                        {this.GetSerializeCoreBlock(t.vtableIndex, t.modelIndex, t.valueVariableName, t.layout, t.model, context)}
                    }}
                    ");
                }
            }

            List<string> body = new();
            body.Add(methodStart);
            body.AddRange(getters);
            body.AddRange(prepareBlocks);

            // We probably over-allocated. Figure out by how much and back up the cursor.
            // Then we can write the vtable.
            body.Add("int tableLength = currentOffset - tableStart;");
            body.Add($"{context.SerializationContextVariableName}.{nameof(SerializationContext.Offset)} -= {maxInlineSize} - tableLength;");

            // Finish vtable.
            body.Add($"{context.SpanWriterVariableName}.{nameof(ISpanWriter.WriteUShort)}(vtable, (ushort)vtableLength, 0, {context.SerializationContextVariableName});");
            body.Add($"{context.SpanWriterVariableName}.{nameof(ISpanWriter.WriteUShort)}(vtable, (ushort)tableLength, sizeof(ushort), {context.SerializationContextVariableName});");

            body.Add($"int vtablePosition = {context.SerializationContextVariableName}.{nameof(SerializationContext.FinishVTable)}({context.SpanVariableName}, vtable.Slice(0, vtableLength));");
            body.Add($"{context.SpanWriterVariableName}.{nameof(SpanWriter.WriteInt)}({context.SpanVariableName}, tableStart - vtablePosition, tableStart, {context.SerializationContextVariableName});");

            body.AddRange(writeBlocks);

            // These methods are often enormous, and inlining can have a detrimental effect on perf.
            return new CodeGeneratedMethod(string.Join("\r\n", body));
        }

        private static string OffsetVariableName(int index, int i) => $"index{index + i}Offset";

        private string GetPrepareSerializeBlock(
            int index,
            int i,
            string valueVariableName,
            PhysicalLayoutElement layout,
            TableMemberModel memberModel,
            SerializationCodeGenContext context)
        {
            string condition = $"if ({memberModel.ItemTypeModel.GetNotEqualToDefaultValueLiteralExpression(valueVariableName, memberModel.DefaultValueLiteral)})";

            if (memberModel.ForceWrite)
            {
                condition = string.Empty;
            }

            int vTableIndex = sizeof(ushort) * (2 + (index + i));
            int vTableLength = vTableIndex + sizeof(ushort);

            string prepareBlock = $@"
                currentOffset += {nameof(SerializationHelpers)}.{nameof(SerializationHelpers.GetAlignmentError)}(currentOffset, {layout.Alignment});
                {OffsetVariableName(index, i)} = currentOffset;
                currentOffset += {layout.InlineSize};";

            string setVtableBlock = string.Empty;
            if (i == memberModel.ItemTypeModel.PhysicalLayout.Length - 1)
            {
                setVtableBlock = $@"
                    if ({vTableLength} > vtableLength)
                    {{
                        vtableLength = {vTableLength};
                    }}";
            }

            string writeVTableBlock = 
                $"{context.SpanWriterVariableName}.{nameof(ISpanWriter.WriteUShort)}(vtable, (ushort)({OffsetVariableName(index, i)} - tableStart), {vTableIndex}, {context.SerializationContextVariableName});";

            string inlineSerialize = string.Empty;
            if (memberModel.ItemTypeModel.SerializesInline)
            {
                inlineSerialize = this.GetSerializeCoreBlock(
                    index, i, valueVariableName, layout, memberModel, context);
            }

            return $@"
                var {OffsetVariableName(index, i)} = tableStart;
                {condition} 
                {{
                    {prepareBlock}
                    {inlineSerialize}
                    {setVtableBlock}
                }}
                {writeVTableBlock}";
        }

        private string GetSerializeCoreBlock(
            int index,
            int i,
            string valueVariableName,
            PhysicalLayoutElement layout,
            TableMemberModel memberModel,
            SerializationCodeGenContext context)
        {
            int vtableEntries = memberModel.ItemTypeModel.PhysicalLayout.Length;

            string sortInvocation = string.Empty;
            if (memberModel.IsSortedVector)
            {
                if (!memberModel.ItemTypeModel.TryGetUnderlyingVectorType(out ITypeModel? tableModel) ||
                    !tableModel.TryGetTableKeyMember(out TableMemberModel? keyMember) ||
                    !keyMember.ItemTypeModel.TryGetSpanComparerType(out Type? spanComparerType) ||
                    keyMember.ItemTypeModel.PhysicalLayout.Length != 1)
                {
                    string? vtm = memberModel.ItemTypeModel.ClrType.FullName;
                    string? ttm = tableModel?.GetType().FullName;
                    string? ttn = tableModel?.ClrType.FullName;

                    throw new InvalidOperationException($"Internal error: Validation failed when writing sorted vector. VTM={vtm}, TTM={ttm}, TTN={ttn}");
                }

                string inlineSize = keyMember.ItemTypeModel.IsFixedSize ? keyMember.ItemTypeModel.PhysicalLayout[0].InlineSize.ToString() : "null";

                sortInvocation = @$"
                    {context.SerializationContextVariableName}.{nameof(SerializationContext.AddPostSerializeAction)}(
                        (tempSpan, ctx) =>
                        {nameof(SortedVectorHelpers)}.{nameof(SortedVectorHelpers.SortVector)}(
                            tempSpan, 
                            {OffsetVariableName(index, 0)}, 
                            {keyMember.Index}, 
                            {inlineSize}, 
                            new {CSharpHelpers.GetCompilableTypeName(spanComparerType)}({keyMember.DefaultValueLiteral})));";
            }

            // NULL FORGIVENESS
            string nullForgiving = string.Empty;
            if (memberModel.ItemTypeModel.ClassifyContextually(this.SchemaType).IsOptionalReference())
            {
                nullForgiving = "!";
            }

            if (vtableEntries == 1)
            {
                return $@"
                    {context.MethodNameMap[memberModel.ItemTypeModel.ClrType]}(
                        {context.SpanWriterVariableName},
                        {context.SpanVariableName},
                        {valueVariableName}{nullForgiving},
                        {OffsetVariableName(index, 0)},
                        {context.SerializationContextVariableName});
                    {sortInvocation}";
            }
            else
            {
                return $@"
                    var offsetTuple = ({string.Join(", ", Enumerable.Range(0, vtableEntries).Select(x => OffsetVariableName(index, x)))});
                    {context.MethodNameMap[memberModel.ItemTypeModel.ClrType]}(
                        {context.SpanWriterVariableName},
                        {context.SpanVariableName},
                        {valueVariableName}{nullForgiving},
                        ref offsetTuple,
                        {context.SerializationContextVariableName});
                    {sortInvocation}";
            }
        }

        public override CodeGeneratedMethod CreateParseMethodBody(ParserCodeGenContext context)
        {
            // We have to implement two items: The table class and the overall "read" method.
            // Let's start with the read method.
            string className = "tableReader_" + Guid.NewGuid().ToString("n");
            var classDef = new DeserializeClassDefinition(className, this, context.Options);

            // Build up a list of property overrides.
            foreach (var item in this.IndexToMemberMap.Where(x => !x.Value.IsDeprecated))
            {
                int index = item.Key;
                var value = item.Value;
                classDef.AddProperty(value, context.MethodNameMap[value.ItemTypeModel.ClrType]);
            }

            string body = $"return new {className}<{context.InputBufferTypeName}>({context.InputBufferVariableName}, {context.OffsetVariableName} + {context.InputBufferVariableName}.{nameof(InputBufferExtensions.ReadUOffset)}({context.OffsetVariableName}));";
            return new CodeGeneratedMethod(body)
            {
                ClassDefinition = classDef.ToString(),
            };
        }

        public override CodeGeneratedMethod CreateGetMaxSizeMethodBody(GetMaxSizeCodeGenContext context)
        {
            int vtableEntryCount = this.MaxIndex + 1;

            // vtable length + table length + 2 * entryCount + padding to 2-byte alignment.
            int maxVtableSize = sizeof(ushort) * (2 + vtableEntryCount) + SerializationHelpers.GetMaxPadding(sizeof(ushort));
            int maxTableSize = this.NonPaddedMaxTableInlineSize + SerializationHelpers.GetMaxPadding(this.PhysicalLayout.Single().Alignment);

            List<string> statements = new List<string>();
            foreach (var kvp in this.IndexToMemberMap.Where(x => !x.Value.IsDeprecated))
            {
                int index = kvp.Key;
                var member = kvp.Value;

                var itemModel = member.ItemTypeModel;

                if (itemModel.IsFixedSize)
                {
                    // This should already be accounted for in table.NonPaddedMax size above.
                    continue;
                }

                string variableName = $"index{index}Value";
                statements.Add($"var {variableName} = {context.ValueVariableName}.{member.PropertyInfo.Name};");

                string statement =
$@" 
                    if ({itemModel.GetNotEqualToDefaultValueLiteralExpression(variableName, member.DefaultValueLiteral)})
                    {{
                        runningSum += {context.MethodNameMap[itemModel.ClrType]}({variableName});
                    }}";

                statements.Add(statement);
            }

            string body =
$@"
            int runningSum = {maxTableSize} + {maxVtableSize};
            {string.Join("\r\n", statements)};
            return runningSum;
";
            return new CodeGeneratedMethod(body);
        }

        public override CodeGeneratedMethod CreateCloneMethodBody(CloneCodeGenContext context)
        {
            string body = $"return {context.ItemVariableName} is null ? null : new {this.GetCompilableTypeName()}({context.ItemVariableName});";
            return new CodeGeneratedMethod(body)
            {
                IsMethodInline = true,
            };
        }

        public override bool TryGetTableKeyMember([NotNullWhen(true)] out TableMemberModel? tableMember)
        {
            tableMember = this.KeyMember;
            return tableMember != null;
        }

        public override void TraverseObjectGraph(HashSet<Type> seenTypes)
        {
            seenTypes.Add(this.ClrType);
            foreach (var member in this.memberTypes.Values)
            {
                if (seenTypes.Add(member.ItemTypeModel.ClrType))
                {
                    member.ItemTypeModel.TraverseObjectGraph(seenTypes);
                }
            }
        }
    }
}

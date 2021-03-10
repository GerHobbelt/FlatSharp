
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the FlatSharp FBS to C# compiler (source hash: 5.0.0.MFUC7zYAroKMlXj1E0kiiJDDgY3oGGxaNlA1ZGfN024=)
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FlatSharp;
using FlatSharp.Attributes;
#pragma warning disable 0618
#nullable enable annotations
namespace FlatSharp.Compiler.Generated
{
    internal static class CloneHelpers_81c56e5c7ccc4e59a37657ed6eaf0e73
    {
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static BenchmarkCore.SortedVectorTable? Clone(BenchmarkCore.SortedVectorTable? item)
        {
            checked
            {
                return item is null ? null : new BenchmarkCore.SortedVectorTable(item);
            }
        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static System.Collections.Generic.IList<BenchmarkCore.SortedVectorTableItem>? Clone(System.Collections.Generic.IList<BenchmarkCore.SortedVectorTableItem>? item)
        {
            checked
            {
                return VectorCloneHelpers.Clone<BenchmarkCore.SortedVectorTableItem>(item, FlatSharp.Compiler.Generated.CloneHelpers_81c56e5c7ccc4e59a37657ed6eaf0e73.Clone);
            }
        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static BenchmarkCore.SortedVectorTableItem? Clone(BenchmarkCore.SortedVectorTableItem? item)
        {
            checked
            {
                return item is null ? null : new BenchmarkCore.SortedVectorTableItem(item);
            }
        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static System.Int32 Clone(System.Int32 item)
        {
            checked
            {
                return item;
            }
        }
    }

}

namespace BenchmarkCore
{
    [FlatBufferTable]
    [System.Runtime.CompilerServices.CompilerGenerated]
    public partial class SortedVectorTable : object
    {
        public SortedVectorTable()
        {
            checked
            {
                this.OnInitialized(null);
            }
        }

#pragma warning disable CS8618
        protected SortedVectorTable(FlatBufferDeserializationContext context)
        {
            checked
            {
                this.OnInitialized(context);
            }
        }

#pragma warning restore CS8618
        public SortedVectorTable(SortedVectorTable source)
        {
            checked
            {
                this.Vector = FlatSharp.Compiler.Generated.CloneHelpers_81c56e5c7ccc4e59a37657ed6eaf0e73.Clone(source.Vector);
                this.OnInitialized(null);
            }
        }

        partial void OnInitialized(FlatBufferDeserializationContext? context);
        [FlatBufferItemAttribute(0, SortedVector = true)]
        public virtual IList<global::BenchmarkCore.SortedVectorTableItem>? Vector { get; set; }

        public static ISerializer<BenchmarkCore.SortedVectorTable> Serializer { get; } = new GeneratedSerializer().AsISerializer();

        #region Serializer for BenchmarkCore.SortedVectorTable

        [FlatSharpGeneratedSerializerAttribute(FlatBufferDeserializationOption.GreedyMutable)]
        private sealed class GeneratedSerializer : IGeneratedSerializer<BenchmarkCore.SortedVectorTable>
        {

            public void Write<TSpanWriter>(TSpanWriter writer, Span<byte> target, BenchmarkCore.SortedVectorTable root, int offset, SerializationContext context)
                where TSpanWriter : ISpanWriter
            {
                checked
                {
                    WriteInlineValueOf_7b5996bec77f4c71928769dd75748853(writer, target, root, offset, context);
                }
            }

            public int GetMaxSize(BenchmarkCore.SortedVectorTable root)
            {
                checked
                {
                    return GetMaxSizeOf_7b5996bec77f4c71928769dd75748853(root);
                }
            }

            public BenchmarkCore.SortedVectorTable Parse<TInputBuffer>(TInputBuffer buffer, int offset)
                where TInputBuffer : IInputBuffer
            {
                checked
                {
                    return Read_7b5996bec77f4c71928769dd75748853(buffer, offset);
                }
            }

            private static int GetMaxSizeOf_7b5996bec77f4c71928769dd75748853(BenchmarkCore.SortedVectorTable value)
            {
                checked
                {

                    int runningSum = 14 + 7;
                    var index0Value = value.Vector;

                    if (!(index0Value is null))
                    {
                        runningSum += GetMaxSizeOf_36d43cc8bc8e4fbd92e5d1e6822467b6(index0Value);
                    };
                    return runningSum;

                }
            }

            private static BenchmarkCore.SortedVectorTable Read_7b5996bec77f4c71928769dd75748853<TInputBuffer>(
                TInputBuffer buffer,
                int offset) where TInputBuffer : IInputBuffer
            {
                checked
                {
                    return new tableReader_0645c26e34c54c24848905fe30fc6839<TInputBuffer>(buffer, offset + buffer.ReadUOffset(offset));
                }
            }

            private sealed class tableReader_0645c26e34c54c24848905fe30fc6839<TInputBuffer>
                : BenchmarkCore.SortedVectorTable
                , IFlatBufferDeserializedObject
                where TInputBuffer : IInputBuffer
            {
                private static readonly FlatBufferDeserializationContext __CtorContext
                    = new FlatBufferDeserializationContext(FlatBufferDeserializationOption.GreedyMutable);

                private System.Collections.Generic.IList<BenchmarkCore.SortedVectorTableItem>? __index0Value;

                public tableReader_0645c26e34c54c24848905fe30fc6839(TInputBuffer buffer, int offset) : base(__CtorContext)
                {
                    checked
                    {
                        buffer.InitializeVTable(offset, out var __vtableLocation, out var __vtableMaxIndex);
                        this.__index0Value = ReadIndex0Value(buffer, offset, __vtableLocation, __vtableMaxIndex);
                    }
                }

                Type IFlatBufferDeserializedObject.TableOrStructType => typeof(BenchmarkCore.SortedVectorTable);
                FlatBufferDeserializationContext IFlatBufferDeserializedObject.DeserializationContext => __CtorContext;
                IInputBuffer? IFlatBufferDeserializedObject.InputBuffer => null;


                public override System.Collections.Generic.IList<BenchmarkCore.SortedVectorTableItem>? Vector
                {
                    get
                    {
                        checked
                        {
                            return this.__index0Value;
                        }
                    }

                    set
                    {
                        checked
                        { this.__index0Value = value; }
                    }
                }


                [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                private static System.Collections.Generic.IList<BenchmarkCore.SortedVectorTableItem>? ReadIndex0Value(
                    TInputBuffer buffer,
                    int offset,
                    int vtableOffset,
                    int maxVtableIndex)
                {
                    checked
                    {

                        if (0 > maxVtableIndex)
                        {
                            return null;
                        }

                        ushort relativeOffset = buffer.ReadUShort(vtableOffset + 4);
                        if (relativeOffset == 0)
                        {
                            return null;
                        }

                        int absoluteLocation = offset + relativeOffset;
                        return Read_36d43cc8bc8e4fbd92e5d1e6822467b6(buffer, absoluteLocation); ;
                    }
                }
            }



            private static void WriteInlineValueOf_7b5996bec77f4c71928769dd75748853<TSpanWriter>(
                TSpanWriter spanWriter,
                Span<byte> span,
                BenchmarkCore.SortedVectorTable value,
                int offset,
                SerializationContext context) where TSpanWriter : ISpanWriter
            {
                checked
                {

                    int tableStart = context.AllocateSpace(11, sizeof(int));
                    spanWriter.WriteUOffset(span, offset, tableStart, context);
                    int currentOffset = tableStart + sizeof(int); // skip past vtable soffset_t.

                    int vtableLength = 4;
                    Span<byte> vtable = stackalloc byte[6];

                    var index0Value = value.Vector;
                    var index0Offset = tableStart;

                    if (!(index0Value is null))
                    {

                        currentOffset += SerializationHelpers.GetAlignmentError(currentOffset, 4);
                        index0Offset = currentOffset;
                        currentOffset += 4;


                        if (6 > vtableLength)
                        {
                            vtableLength = 6;
                        }
                    }
                    spanWriter.WriteUShort(vtable, (ushort)(index0Offset - tableStart), 4, context);
                    int tableLength = currentOffset - tableStart;
                    context.Offset -= 11 - tableLength;
                    spanWriter.WriteUShort(vtable, (ushort)vtableLength, 0, context);
                    spanWriter.WriteUShort(vtable, (ushort)tableLength, sizeof(ushort), context);
                    int vtablePosition = context.FinishVTable(span, vtable.Slice(0, vtableLength));
                    spanWriter.WriteInt(span, tableStart - vtablePosition, tableStart, context);

                    if (index0Offset != tableStart)
                    {

                        WriteInlineValueOf_36d43cc8bc8e4fbd92e5d1e6822467b6(
                            spanWriter,
                            span,
                            index0Value!,
                            index0Offset,
                            context);

                        context.AddPostSerializeAction(
                            (tempSpan, ctx) =>
                            SortedVectorHelpers.SortVector(
                                tempSpan,
                                index0Offset,
                                0,
                                4,
                                new FlatSharp.IntSpanComparer(default(System.Int32))));
                    }

                }
            }

            private static int GetMaxSizeOf_36d43cc8bc8e4fbd92e5d1e6822467b6(System.Collections.Generic.IList<BenchmarkCore.SortedVectorTableItem> value)
            {
                checked
                {

                    int length = value.Count;
                    int runningSum = 7 + 7;
                    for (int i = 0; i < length; ++i)
                    {
                        var itemTemp = value[i];
                        SerializationHelpers.EnsureNonNull(itemTemp);
                        runningSum += GetMaxSizeOf_96c3a6fa438049bf89a78192c2c3e4aa(itemTemp);
                    }
                    return runningSum;
                }
            }

            private static System.Collections.Generic.IList<BenchmarkCore.SortedVectorTableItem> Read_36d43cc8bc8e4fbd92e5d1e6822467b6<TInputBuffer>(
                TInputBuffer buffer,
                int offset) where TInputBuffer : IInputBuffer
            {
                checked
                {
                    return (new FlatBufferVector_83577285241a42dd90e7097db71706f4<TInputBuffer>(
                        buffer,
                        offset + buffer.ReadUOffset(offset),
                        4)).FlatBufferVectorToList();
                }
            }

            public sealed class FlatBufferVector_83577285241a42dd90e7097db71706f4<TInputBuffer> : FlatBufferVector<BenchmarkCore.SortedVectorTableItem, TInputBuffer>
                where TInputBuffer : IInputBuffer
            {
                public FlatBufferVector_83577285241a42dd90e7097db71706f4(
                    TInputBuffer memory,
                    int offset,
                    int itemSize) : base(memory, offset, itemSize)
                {
                    checked
                    {
                    }
                }

                protected override BenchmarkCore.SortedVectorTableItem ParseItem(TInputBuffer memory, int offset)
                {
                    checked
                    {
                        return Read_96c3a6fa438049bf89a78192c2c3e4aa(memory, offset);
                    }
                }
            }



            private static void WriteInlineValueOf_36d43cc8bc8e4fbd92e5d1e6822467b6<TSpanWriter>(
                TSpanWriter spanWriter,
                Span<byte> span,
                System.Collections.Generic.IList<BenchmarkCore.SortedVectorTableItem> value,
                int offset,
                SerializationContext context) where TSpanWriter : ISpanWriter
            {
                checked
                {

                    int count = value.Count;
                    int vectorOffset = context.AllocateVector(4, count, 4);
                    spanWriter.WriteUOffset(span, offset, vectorOffset, context);
                    spanWriter.WriteInt(span, count, vectorOffset, context);
                    vectorOffset += sizeof(int);
                    for (int i = 0; i < count; ++i)
                    {
                        var current = value[i];
                        SerializationHelpers.EnsureNonNull(current);
                        WriteInlineValueOf_96c3a6fa438049bf89a78192c2c3e4aa(spanWriter, span, current, vectorOffset, context);
                        vectorOffset += 4;
                    }
                }
            }

            private static int GetMaxSizeOf_96c3a6fa438049bf89a78192c2c3e4aa(BenchmarkCore.SortedVectorTableItem value)
            {
                checked
                {

                    int runningSum = 14 + 7;
                    ;
                    return runningSum;

                }
            }

            private static BenchmarkCore.SortedVectorTableItem Read_96c3a6fa438049bf89a78192c2c3e4aa<TInputBuffer>(
                TInputBuffer buffer,
                int offset) where TInputBuffer : IInputBuffer
            {
                checked
                {
                    return new tableReader_c7a283deb4ff4fa99ffb71502c4c2990<TInputBuffer>(buffer, offset + buffer.ReadUOffset(offset));
                }
            }

            private sealed class tableReader_c7a283deb4ff4fa99ffb71502c4c2990<TInputBuffer>
                : BenchmarkCore.SortedVectorTableItem
                , IFlatBufferDeserializedObject
                where TInputBuffer : IInputBuffer
            {
                private static readonly FlatBufferDeserializationContext __CtorContext
                    = new FlatBufferDeserializationContext(FlatBufferDeserializationOption.GreedyMutable);

                private System.Int32 __index0Value;

                public tableReader_c7a283deb4ff4fa99ffb71502c4c2990(TInputBuffer buffer, int offset) : base(__CtorContext)
                {
                    checked
                    {
                        buffer.InitializeVTable(offset, out var __vtableLocation, out var __vtableMaxIndex);
                        this.__index0Value = ReadIndex0Value(buffer, offset, __vtableLocation, __vtableMaxIndex);
                    }
                }

                Type IFlatBufferDeserializedObject.TableOrStructType => typeof(BenchmarkCore.SortedVectorTableItem);
                FlatBufferDeserializationContext IFlatBufferDeserializedObject.DeserializationContext => __CtorContext;
                IInputBuffer? IFlatBufferDeserializedObject.InputBuffer => null;


                public override System.Int32 Key
                {
                    get
                    {
                        checked
                        {
                            return this.__index0Value;
                        }
                    }

                    set
                    {
                        checked
                        { this.__index0Value = value; }
                    }
                }


                [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                private static System.Int32 ReadIndex0Value(
                    TInputBuffer buffer,
                    int offset,
                    int vtableOffset,
                    int maxVtableIndex)
                {
                    checked
                    {

                        if (0 > maxVtableIndex)
                        {
                            return default(System.Int32);
                        }

                        ushort relativeOffset = buffer.ReadUShort(vtableOffset + 4);
                        if (relativeOffset == 0)
                        {
                            return default(System.Int32);
                        }

                        int absoluteLocation = offset + relativeOffset;
                        return Read_2c1d694fadee47f08ebfecc7f8090de6(buffer, absoluteLocation); ;
                    }
                }
            }



            private static void WriteInlineValueOf_96c3a6fa438049bf89a78192c2c3e4aa<TSpanWriter>(
                TSpanWriter spanWriter,
                Span<byte> span,
                BenchmarkCore.SortedVectorTableItem value,
                int offset,
                SerializationContext context) where TSpanWriter : ISpanWriter
            {
                checked
                {

                    int tableStart = context.AllocateSpace(11, sizeof(int));
                    spanWriter.WriteUOffset(span, offset, tableStart, context);
                    int currentOffset = tableStart + sizeof(int); // skip past vtable soffset_t.

                    int vtableLength = 4;
                    Span<byte> vtable = stackalloc byte[6];

                    var index0Value = value.Key;
                    var index0Offset = tableStart;

                    if (index0Value != default(System.Int32))
                    {

                        currentOffset += SerializationHelpers.GetAlignmentError(currentOffset, 4);
                        index0Offset = currentOffset;
                        currentOffset += 4;

                        WriteInlineValueOf_2c1d694fadee47f08ebfecc7f8090de6(
                            spanWriter,
                            span,
                            index0Value,
                            index0Offset,
                            context);


                        if (6 > vtableLength)
                        {
                            vtableLength = 6;
                        }
                    }
                    spanWriter.WriteUShort(vtable, (ushort)(index0Offset - tableStart), 4, context);
                    int tableLength = currentOffset - tableStart;
                    context.Offset -= 11 - tableLength;
                    spanWriter.WriteUShort(vtable, (ushort)vtableLength, 0, context);
                    spanWriter.WriteUShort(vtable, (ushort)tableLength, sizeof(ushort), context);
                    int vtablePosition = context.FinishVTable(span, vtable.Slice(0, vtableLength));
                    spanWriter.WriteInt(span, tableStart - vtablePosition, tableStart, context);
                }
            }

            [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            private static int GetMaxSizeOf_2c1d694fadee47f08ebfecc7f8090de6(System.Int32 value)
            {
                checked
                {
                    return 7;
                }
            }

            [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            private static System.Int32 Read_2c1d694fadee47f08ebfecc7f8090de6<TInputBuffer>(
                TInputBuffer buffer,
                int offset) where TInputBuffer : IInputBuffer
            {
                checked
                {
                    return buffer.ReadInt(offset);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void WriteInlineValueOf_2c1d694fadee47f08ebfecc7f8090de6<TSpanWriter>(
                TSpanWriter spanWriter,
                Span<byte> span,
                System.Int32 value,
                int offset,
                SerializationContext context) where TSpanWriter : ISpanWriter
            {
                checked
                {
                    spanWriter.WriteInt(span, value, offset, context);
                }
            }
        }

        #endregion
    }
    [FlatBufferTable]
    [System.Runtime.CompilerServices.CompilerGenerated]
    public partial class SortedVectorTableItem : object
    {
        public SortedVectorTableItem()
        {
            checked
            {
                this.OnInitialized(null);
            }
        }

#pragma warning disable CS8618
        protected SortedVectorTableItem(FlatBufferDeserializationContext context)
        {
            checked
            {
                this.OnInitialized(context);
            }
        }

#pragma warning restore CS8618
        public SortedVectorTableItem(SortedVectorTableItem source)
        {
            checked
            {
                this.Key = FlatSharp.Compiler.Generated.CloneHelpers_81c56e5c7ccc4e59a37657ed6eaf0e73.Clone(source.Key);
                this.OnInitialized(null);
            }
        }

        partial void OnInitialized(FlatBufferDeserializationContext? context);
        [FlatBufferItemAttribute(0, Key = true)]
        public virtual System.Int32 Key { get; set; }

    }
}
#nullable restore
#pragma warning restore 0618

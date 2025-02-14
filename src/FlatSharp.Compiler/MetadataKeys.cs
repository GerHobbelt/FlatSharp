﻿/*
 * Copyright 2021 James Courtney
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

namespace FlatSharp.Compiler
{
    using System.Collections.Generic;

    /// <summary>
    /// Names of attributes specified in FBS files.
    /// </summary>
    public static class MetadataKeys
    {
        /// <summary>
        /// Controls the type of serializer generated for the given table.
        /// Valid On:
        /// - Table
        /// </summary>
        public const string SerializerKind = "fs_serializer";
        public const string PrecompiledSerializerLegacy = "PrecompiledSerializer";

        /// <summary>
        /// Controls whether fields are virtual or non-virtual. They are virtual by default.
        /// Valid On:
        /// - Table (as a default for all fields in the table, overridden by the setting on a field)
        /// - Struct (as a default for all fields in the struct, overridden by the setting on a field)
        /// - Field 
        /// </summary>
        public const string NonVirtualProperty = "fs_nonVirtual";
        public const string NonVirtualPropertyLegacy = "nonVirtual";

        /// <summary>
        /// Controls whether a vector field should be sorted or not. The field must be a vector.
        /// Valid On:
        /// - Field 
        /// </summary>
        public const string SortedVector = "fs_sortedVector";
        public const string SortedVectorLegacy = "sortedvector";

        /// <summary>
        /// Controls whether FlatSharp should model the string or string vector as a SharedString for string deduplication.
        /// Valid On:
        /// - String Field or Vector of String field
        /// </summary>
        public const string SharedString = "fs_sharedString";
        public const string SharedStringLegacy = "sharedstring";

        /// <summary>
        /// Controls how FlatSharp should generate a default constructor for the given type.
        /// </summary>
        public const string DefaultConstructorKind = "fs_defaultCtor";
        public const string ObsoleteDefaultConstructorLegacy = "ObsoleteDefaultConstructor";

        /// <summary>
        /// Controls the type of vector FlatSharp will generate. Valid values can be found in <see cref="VectorType"/>.
        /// Valid On:
        /// - Table vector field
        /// </summary>
        public const string VectorKind = "fs_vector";
        public const string VectorKindLegacy = "VectorType";

        /// <summary>
        /// Controls the type of setter FlatSharp will generate for a given field. Valid values can be found in <see cref="SetterKind"/>.
        /// Valid On:
        /// - Table field
        /// - Struct field
        /// </summary>
        public const string Setter = "fs_setter";
        public const string SetterLegacy = "setter";

        /// <summary>
        /// Controls the file identifier injected into the buffer when serializing. Must be precisely a 4 byte ASCII string.
        /// </summary>
        public const string FileIdentifier = "fs_fileId";

        /// <summary>
        /// Indicates that a single table field should be force-written, or when declared on a table
        /// that all fields should be force written. Force Write refers to writing a value even if it
        /// matches the default.
        /// </summary>
        public const string ForceWrite = "fs_forceWrite";

        /// <summary>
        /// Marks a table field as deprecated. Deprecated fields do not have their values serialized or parsed.
        /// Valid On:
        /// - Table field
        /// </summary>
        public const string Deprecated = "deprecated";

        /// <summary>
        /// Marks a table field as being the key to a sorted vector.
        /// Valid On:
        /// - Table field, with the type of string or scalar.
        /// </summary>
        public const string Key = "key";

        /// <summary>
        /// Defines an explicit ID for a table field, so fields can be out-of-order in the FBS file.
        /// Valid On:
        /// - Table field
        /// </summary>
        public const string Id = "id";

        public static IEnumerable<string> UnsupportedStandardAttributes => new[]
        {
            "required", "force_align", "bit_flags", "flexbuffer", "hash", "original_order"
        };
    }
}

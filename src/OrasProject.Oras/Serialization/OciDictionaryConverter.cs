// Copyright The ORAS Authors.
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OrasProject.Oras.Serialization;

/// <summary>
/// OciDictionaryConverter serializes IDictionary&lt;string, string&gt;
/// with Go-compatible escaping for both keys and values.
/// JsonConverter&lt;string&gt; is not called for dictionary keys,
/// so this converter handles the full dictionary serialization.
/// </summary>
internal sealed class OciDictionaryConverter
    : JsonConverter<IDictionary<string, string>>
{
    public override IDictionary<string, string>? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var dict = new Dictionary<string, string>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dict;
            }

            var key = reader.GetString()!;
            reader.Read();
            dict[key] = reader.GetString()!;
        }

        return dict;
    }

    public override void Write(
        Utf8JsonWriter writer,
        IDictionary<string, string> value,
        JsonSerializerOptions options)
    {
        var sb = new StringBuilder();
        sb.Append('{');
        var first = true;
        foreach (var kvp in value)
        {
            if (!first)
            {
                sb.Append(',');
            }
            first = false;
            sb.Append('"');
            sb.Append(OciJsonSerializer.EscapeJsonString(kvp.Key));
            sb.Append("\":\"");
            sb.Append(OciJsonSerializer.EscapeJsonString(kvp.Value));
            sb.Append('"');
        }
        sb.Append('}');
        writer.WriteRawValue(sb.ToString());
    }
}

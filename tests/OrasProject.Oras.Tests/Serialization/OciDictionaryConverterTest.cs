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

using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using OrasProject.Oras.Oci;
using OrasProject.Oras.Serialization;
using Xunit;

namespace OrasProject.Oras.Tests.Serialization;

public class OciDictionaryConverterTest
{
    private static string SerializeDict(
        IDictionary<string, string> dict)
    {
        var bytes = OciJsonSerializer.SerializeToUtf8Bytes(dict);
        return Encoding.UTF8.GetString(bytes);
    }

    [Theory]
    [InlineData("k+ey", "val+ue", "+", "\\u002B")]
    [InlineData("k<ey", "val&ue", "\\u003C", "<")]
    [InlineData("k\u2028", "v\u2029", "\\u2028", null)]
    public void Serialize_ShouldEscapeKeysAndValues(
        string key, string value, string expected, string? forbidden)
    {
        var dict = new Dictionary<string, string> { [key] = value };
        var json = SerializeDict(dict);
        Assert.Contains(expected, json);
        if (forbidden != null)
        {
            Assert.DoesNotContain(forbidden, json);
        }
    }

    [Fact]
    public void EmptyDictionary_ShouldSerializeAsEmptyObject()
    {
        var json = SerializeDict(new Dictionary<string, string>());
        Assert.Equal("{}", json);
    }

    [Fact]
    public void Deserialize_NullAnnotations_ReturnsNull()
    {
        var json = """
            {
              "schemaVersion": 2,
              "mediaType": "application/vnd.oci.image.manifest.v1+json",
              "config": {
                "mediaType": "application/vnd.oci.empty.v1+json",
                "digest": "sha256:44136fa355b311bfa706c319d8f39c36e47d288aca2e1cc38b1c",
                "size": 2
              },
              "layers": [],
              "annotations": null
            }
            """;
        var manifest = OciJsonSerializer.Deserialize<Manifest>(
            Encoding.UTF8.GetBytes(json))!;
        Assert.Null(manifest.Annotations);
    }

    [Theory]
    [InlineData("""{"annotations": {"k": 123}}""")]
    [InlineData("""{"annotations": {"k": [1,2]}}""")]
    [InlineData("""{"annotations": {"k": {"n": true}}}""")]
    public void Deserialize_NonStringAnnotationValue_Throws(
        string json)
    {
        Assert.ThrowsAny<JsonException>(() =>
            OciJsonSerializer.Deserialize<Manifest>(
                Encoding.UTF8.GetBytes(json)));
    }

    [Theory]
    [MemberData(nameof(RoundTripData))]
    public void RoundTrip_ShouldPreserveKeyValues(
        Dictionary<string, string> dict)
    {
        var bytes = OciJsonSerializer.SerializeToUtf8Bytes(dict);
        var result = OciJsonSerializer
            .Deserialize<Dictionary<string, string>>(bytes);
        Assert.NotNull(result);
        foreach (var kvp in dict)
        {
            Assert.Equal(kvp.Value, result![kvp.Key]);
        }
    }

    public static IEnumerable<object[]> RoundTripData()
    {
        yield return new object[]
        {
            new Dictionary<string, string>
            {
                ["org.example+custom"] = "value+plus"
            }
        };
        yield return new object[]
        {
            new Dictionary<string, string>
            {
                ["<html>"] = "&value",
                ["normal"] = "normal"
            }
        };
    }
}

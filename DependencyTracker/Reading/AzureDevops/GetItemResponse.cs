using System.Text.Json.Serialization;

namespace DependencyTracker.Reading.AzureDevops;

internal sealed class GetItemResponse
{
    [JsonPropertyName("count")] public int Count { get; set; }
    [JsonPropertyName("value")] public List<Value> Values { get; set; } = [];

    public class Value
    {
        [JsonPropertyName("objectId")] public string? ObjectId { get; set; }
        [JsonPropertyName("gitObjectType")] public string? GitObjectType { get; set; }
        [JsonPropertyName("commitId")] public string? CommitId { get; set; }
        [JsonPropertyName("path")] public string Path { get; set; } = default!;

        [JsonPropertyName("isFolder")] public bool? IsFolder { get; set; }
        [JsonPropertyName("url")] public string Url { get; set; } = default!;
    }
}
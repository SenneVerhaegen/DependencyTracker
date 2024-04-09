using System.Text.Json.Serialization;

namespace DependencyTracker.Reading.GitHub;

public static class GitHubResponse
{
    public class GitHubTree
    {
        [JsonPropertyName("sha")] public string Sha { get; set; }
        [JsonPropertyName("url")] public string Url { get; set; }
        [JsonPropertyName("tree")] public List<TreeItem> Tree { get; set; }
        [JsonPropertyName("truncated")] public bool Truncated { get; set; }
    }

    public class TreeItem
    {
        [JsonPropertyName("path")] public string Path { get; set; }
        [JsonPropertyName("mode")] public string Mode { get; set; }
        [JsonPropertyName("type")] public string Type { get; set; }
        [JsonPropertyName("size")] public int? Size { get; set; }
        [JsonPropertyName("sha")] public string Sha { get; set; }
        [JsonPropertyName("url")] public string Url { get; set; }
    }
}
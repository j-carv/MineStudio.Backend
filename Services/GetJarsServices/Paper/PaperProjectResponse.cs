using System.Text.Json.Serialization;

namespace MineStudio.Agent.Services.GetJarsServices;

public sealed class PaperProjectResponse
{
    [JsonPropertyName("versions")] public List<string> Versions { get; init; } = [];
}
public sealed class PaperBuildResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("channel")]
    public string Channel { get; set; } = "";

    [JsonPropertyName("downloads")]
    public Dictionary<string, PaperDownloadInfo> Downloads { get; set; } = [];
}

public sealed class PaperDownloadInfo
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = "";
}
namespace MineStudio.Agent.Services.GetJarsServices;

public class PaperService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private HttpClient client;
    
    
    public PaperService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        client = _httpClientFactory.CreateClient("papermc");
    }

    public async Task<List<string>> GetVersionsAsync()
    {

        var response = await client.GetFromJsonAsync<PaperProjectResponse>("projets/paper");

        return response?.Versions ?? [];
    }

    public async Task<int> GetLatestStableBuildAsync(string version)
    {

        var builds = await client.GetFromJsonAsync<List<PaperBuildResponse>>($"projets/paper/versions/{version}/builds");

        if (builds is null || builds.Count == 0)
            throw new Exception($"No builds found for Paper {version}");

        var stableBuild = builds
            .Where(x => x.Channel.Equals("STABLE", StringComparison.OrdinalIgnoreCase))
            .MaxBy(x => x.Id);

        stableBuild ??= builds.MaxBy(x => x.Id);

        return stableBuild!.Id;
    }
    
    public async Task<string> GetDownloadUrlAsync(string version)
    {
        var build = await GetLatestStableBuildAsync(version);

        var fileName = $"paper-{version}-{build}.jar";

        return $"https://fill.papermc.io/v3/projects/paper/versions/{version}/builds/{build}/downloads/{fileName}";
    }

    public async Task DownloadPaperAsync(string version, string destinationPath)
    {
        var client = _httpClientFactory.CreateClient("papermc");
        var url = await GetDownloadUrlAsync(version);

        await using var stream = await client.GetStreamAsync(url);
        await using var file = File.Create(destinationPath);

        await stream.CopyToAsync(file);
    }
}
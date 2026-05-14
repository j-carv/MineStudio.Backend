using MineStudio.Agent.Models.Creation;
using MineStudio.Agent.Services.GetJarsServices;

namespace MineStudio.Agent.Services;

public class ServerCreationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    public ServerCreationService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task CreateServerAsync(CreateServerRequest request)
    {
        var basePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MineStudio",
            "Servers",
            request.Name
        );
        
        Directory.CreateDirectory(basePath);
        
        if (request.Type == "paper")
        {
            Directory.CreateDirectory(Path.Combine(basePath, "plugins"));
        }
        
        if (request.Type is "fabric" or "forge")
        {
            Directory.CreateDirectory(Path.Combine(basePath, "mods"));
        }

        var jarUrl = request.Type.ToLower() switch
        {
            "vanilla" => await GetVanillaJarUrl(request.Version),
            "paper" => await GetPaperJarUrl(request.Version),
            "fabric" => await GetFabricJarUrl(request.Version),
            "forge" => await GetForgeInstallerUrl(request.Version),
            _ => throw new Exception("Invalid Server Type")
        };
        
        var jarPath = Path.Combine(basePath, "server.jar");

        Console.WriteLine($"Downloading server from: {jarUrl}");
        await DownloadFileAsync(jarUrl, jarPath);
        
        await File.WriteAllTextAsync(Path.Combine(basePath, "eula.txt"),"eula=true");

        await File.WriteAllTextAsync(Path.Combine(basePath, "server.properties"),"server-port=25565 online-mode=true motd=MineStudio Server max-players=20");

        await File.WriteAllTextAsync(Path.Combine(basePath, "start.bat"),
            $"java -Xms{request.MemoryMb}M -Xmx{request.MemoryMb}M -jar server.jar nogui pause");
    }

    private async Task DownloadFileAsync(string url, string destination)
    {
        var client = _httpClientFactory.CreateClient();
        
        await using var stream = await client.GetStreamAsync(url);
        await using var file = File.Create(destination);

        await stream.CopyToAsync(file);
    }

    private Task<string> GetVanillaJarUrl(string version)
    {
        throw new NotImplementedException();
    }
    
    private async Task<string> GetPaperJarUrl(string version)
    {
        var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.UserAgent.ParseAdd("MineStudio/1.0");

        var builds = await client.GetFromJsonAsync<List<PaperBuildResponse>>(
            $"https://fill.papermc.io/v3/projects/paper/versions/{version}/builds"
        );

        if (builds is null || builds.Count == 0)
            throw new Exception($"No builds found for Paper {version}");

        var latestBuild = builds
            .Where(x => x.Channel.Equals("STABLE", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.Id)
            .FirstOrDefault();

        if (latestBuild is null)
            throw new Exception($"No STABLE builds found for Paper {version}");

        if (!latestBuild.Downloads.TryGetValue("server:default", out var download))
            throw new Exception($"Download server: default not found for Paper {version}");

        return download.Url;
    }
    
    private Task<string> GetFabricJarUrl(string version)
    {
        throw new NotImplementedException();
    }
    
    private Task<string> GetForgeInstallerUrl(string version)
    {
        throw new NotImplementedException();
    }
}
using MineStudio.Agent.Models.Configurations;

namespace MineStudio.Agent.Services;

public class ServerPropertiesService
{
    public List<ServerPropertiesDTO> Read(string serverPath)
    {
        var filePath = Path.Combine(serverPath, "server.properties");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"server.properties not found on {serverPath}");

        var result = new List<ServerPropertiesDTO>();

        foreach (var line in File.ReadAllLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.TrimStart().StartsWith("#")) continue;
            
            var index = line.IndexOf("=");

            if (index <= 0) continue;
            
            result.Add(new ServerPropertiesDTO
            {
                Key = line[..index].Trim(),
                Value = line[(index + 1)..].Trim()
            });
        }

        return result;
    }

    public void Save(string serverPath, List<ServerPropertiesDTO> properties)
    {
        var filePath = Path.Combine(serverPath, "server.properties");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"server.properties not found from {serverPath}");

        var existingLines = File.ReadAllLines(filePath).ToList();

        for (var i = 0; i < existingLines.Count; i++)
        {
            var line = existingLines[i];

            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.TrimStart().StartsWith("#")) continue;
            
            var index = line.IndexOf("=");
            
            if (index <= 0) continue;
            
            var key = line[..index].Trim();
            var updated = properties.FirstOrDefault(x => x.Key == key);

            if (updated is not null)
                existingLines[i] = $"{key}={updated.Value}";
        }
        
        File.WriteAllLines(filePath, existingLines);
    }
}
namespace MineStudio.Agent.Models.Creation;

public class CreateServerRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public int MemoryMb { get; set; } = 4096;
}
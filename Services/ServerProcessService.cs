using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using MineStudio.Agent.Hubs;

namespace MineStudio.Agent.Services;

public class ServerProcessService
{
    private readonly IHubContext<MinecraftConsoleHub> hubContext;
    
    private Process? process;
    private DateTime? startedAt;
    public bool IsRunning => process is { HasExited: false };
    private readonly List<string> logs = new();

    public ServerProcessService(IHubContext<MinecraftConsoleHub> hubContext)
    {
        this.hubContext = hubContext;    
    }
    public Task StartAsync(string serverPath)
    {
        if (IsRunning) return Task.CompletedTask;

        if (!Directory.Exists(serverPath))
            throw new DirectoryNotFoundException($"The specified path {serverPath} was not found.");
        
        var jarPath = Path.Combine(serverPath, "server.jar");

        if (!File.Exists(jarPath))
            throw new FileNotFoundException($"The specified path {jarPath} was not found.");
        
        process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "java",
                Arguments = "-Xmx4G -Xms4G -jar server.jar nogui",
                WorkingDirectory = serverPath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false
            }
        };

        process.OutputDataReceived += async (s, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                var line = "[MC] " + e.Data;

                logs.Add(line);
                Console.WriteLine(line);

                await hubContext.Clients.All.SendAsync("ConsoleLine", line);
            }
        };

        process.ErrorDataReceived += async (s, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                var line = "[MC ERROR] " + e.Data;

                logs.Add(line);
                Console.WriteLine(line);

                await hubContext.Clients.All.SendAsync("ConsoleLine", line);
            }
        };

        process.Exited += (s, e) =>
        {
            Console.WriteLine("[MC] Process exited. ExitCode: " + process.ExitCode);
            startedAt = null;
        };
        
        process.Start();
        
        startedAt = DateTime.Now;
        
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        
        Console.WriteLine("[MC] Server Started from MineStudio");
        
        return Task.CompletedTask;
    }

    public IReadOnlyList<string> GetLogs()
    {
        return logs.TakeLast(200).ToList();
    }

    public async Task SendCommandAsync(string command)
    {
        if (!IsRunning)
            throw new InvalidOperationException("Server is not running");

        Console.WriteLine($"[MC] {command}");
        if (string.IsNullOrEmpty(command))
            throw new ArgumentException("Invalid command");
        
        await process!.StandardInput.WriteLineAsync(command);
        await process.StandardInput.FlushAsync();
        
        logs.Add($"[MINESTUDIO COMMAND] {command}");
        
        await hubContext.Clients.All.SendAsync("ConsoleLine", "[MINESTUDIO COMMAND] " + command);
    }

    public object GetStatus()
    {
        return new
        {
            running = IsRunning,
            pid = IsRunning ? process?.Id : null,
            startedAt,
            uptime = startedAt is null ? null : DateTime.Now - startedAt
        };
    }

    public async Task GetUptime(CancellationToken ct = default)
    {
        while (IsRunning && !ct.IsCancellationRequested)
        {
            await Task.Delay(1000, ct);
            await hubContext.Clients.All.SendAsync("Uptime",  startedAt is null ? null : DateTime.Now - startedAt);
        }
    }

    public async Task StopAsync(CancellationToken ct = default)
    {
        if (IsRunning)
        {
            await process.StandardInput.WriteLineAsync("stop");
            await process.StandardInput.FlushAsync();
            logs.Clear();
        }
    }
}

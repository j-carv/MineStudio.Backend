using Microsoft.AspNetCore.Mvc;
using MineStudio.Agent.Services;

namespace MineStudio.Agent.Controllers;

[ApiController]
[Route("api/server")]
public class ServerController : ControllerBase
{
    private readonly ServerProcessService server;

    public ServerController(ServerProcessService server)
    {
        this.server = server;
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start()
    {
        // just for debugging, not the final path, it should be universal / abstract on production
        var serverPath = @"C:\Users\joaop\OneDrive\Documentos\MineStudio\MinecraftServerDebug";
        
        await server.StartAsync(serverPath);

        return Ok(new
        {
            message = "Server Started",
            running = server.IsRunning
        });
    }

    [HttpGet("status")]
    public IActionResult Status()
    {
        return Ok(server.GetStatus());
    }

    [HttpGet("logs")]
    public IActionResult Logs()
    {
        return Ok(server.GetLogs());
    }

    [HttpPost("command")]
    public async Task<IActionResult> Command([FromBody] CommandRequest request)
    {
        await server.SendCommandAsync(request.Command);

        return Ok(new
        {
            message = "Command Sent",
            command = request.Command
        });
    }

    [HttpPost("stop")]
    public async Task<IActionResult> Stop()
    {
        await server.StopAsync();
        return Ok();
    }
}

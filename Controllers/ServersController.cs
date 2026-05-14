using Microsoft.AspNetCore.Mvc;
using MineStudio.Agent.Models.Creation;
using MineStudio.Agent.Services;

namespace MineStudio.Agent.Controllers;

[ApiController]
[Route("api/servers")]
public class ServersController : ControllerBase
{
    private readonly ServerCreationService _serverCreationService;

    public ServersController(ServerCreationService serverCreationService)
    {
        _serverCreationService = serverCreationService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateServer([FromBody] CreateServerRequest request)
    {
        await _serverCreationService.CreateServerAsync(request);
        return Ok(new { message = "Server Created Successfully" });
    }
}
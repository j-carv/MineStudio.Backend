using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MineStudio.Agent.Models.Creation;
using MineStudio.Agent.Services;

namespace MineStudio.Agent.Controllers;

[ApiController]
[Route("api/servers/create")]
public class CreateServerController : ControllerBase
{
    private readonly ServerCreationService service;

    public CreateServerController(ServerCreationService service)
    {
        this.service = service;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateServer([FromBody] CreateServerRequest request)
    {
        await service.CreateServerAsync(request);
        return Ok();
    }
}
using Microsoft.AspNetCore.Mvc;
using MineStudio.Agent.Services.GetJarsServices;

namespace MineStudio.Agent.Controllers;

[ApiController]
[Route("api/paper")]
public sealed class PaperController : ControllerBase
{
    private readonly PaperService _paperService;

    public PaperController(PaperService paperService)
    {
        _paperService = paperService;
    }

    [HttpGet("versions")]
    public async Task<IActionResult> GetVersions()
    {
        var versions = await _paperService.GetVersionsAsync();
        return Ok(versions);
    }

    [HttpGet("versions/{version}/latest-build")]
    public async Task<IActionResult> GetLatestBuild(string version)
    {
        var build = await _paperService.GetLatestStableBuildAsync(version);
        return Ok(new { version, build });
    }

    [HttpGet("versions/{version}/download-url")]
    public async Task<IActionResult> GetDownloadUrl(string version)
    {
        var url = await _paperService.GetDownloadUrlAsync(version);
        return Ok(new { url });
    }
}
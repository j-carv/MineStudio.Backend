using Microsoft.AspNetCore.Mvc;
using MineStudio.Agent.Models.Configurations;
using MineStudio.Agent.Services;

namespace MineStudio.Agent.Controllers;

[ApiController]
[Route("api/server/properties")]
public class ServerPropertiesController : ControllerBase
{
    private readonly ServerPropertiesService propertiesService;

    // not the final path, just for debugging, it should be universal / abstract on production
    private readonly string serverPath = $"C:\\Users\\joaop\\OneDrive\\Documentos\\MineStudio\\MinecraftServerDebug";
    
    public ServerPropertiesController(ServerPropertiesService propertiesService)
    {
        this.propertiesService = propertiesService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var properties = propertiesService.Read(serverPath);
        
        return Ok(properties);
    }

    [HttpPut]
    public IActionResult Save([FromBody] List<ServerPropertiesDTO> properties)
    {
        propertiesService.Save(serverPath, properties);

        return Ok(new
        {
            message = "server.properties saved successfully"
        });
    }
}
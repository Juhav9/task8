using Microsoft.AspNetCore.Mvc;
using BlazorSignalRApp.Shared;
using BlazorSignalRApp.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorSignalRApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AppDataController : ControllerBase
{
    private readonly ILogger<AppDataController> _logger;
    private readonly AppDataContext _context;

    public AppDataController(ILogger<AppDataController> logger, AppDataContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("messages")]
    public async Task<IActionResult> GetMessages()
    {
        var model = await _context.Messages.ToListAsync();
        return Ok(model);
    }

    [HttpGet("weatherobservations")]
    public async Task<IActionResult> GetObservations()
    {
        // TASK: populate model with proper observation data from the db
        var model = await _context.Observations.ToListAsync();
                
        return Ok(model);
    }

}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize(Roles = "admin,vendor,csr")]
[ApiController]
[Route("api/v1/[controller]")]
public class WebOnlyController : ControllerBase
{
    private readonly ILogger<WebOnlyController> _logger;

    public WebOnlyController(ILogger<WebOnlyController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "WebOnly")]
    public IActionResult Get()
    {
        // Get the identity of the user making the request
        var identity = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // name of the user
        var name = User.FindFirstValue(ClaimTypes.Name);

        // email of the user
        var email = User.FindFirstValue(ClaimTypes.Email);

        // role of the user
        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new
        {
            identity,
            name,
            email,
            role
        });
    }
}
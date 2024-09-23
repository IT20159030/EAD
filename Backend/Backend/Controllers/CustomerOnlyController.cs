using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize(Roles = "customer")]
[ApiController]
[Route("api/v1/[controller]")]
public class CustomerOnlyController : ControllerBase
{
    private readonly ILogger<CustomerOnlyController> _logger;

    public CustomerOnlyController(ILogger<CustomerOnlyController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "MobileOnly")]
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
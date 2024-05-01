using Microsoft.AspNetCore.Mvc;

namespace Assistant_Kira.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult TestCommand([FromBody] string command)
    {
        return Ok();
    }
}

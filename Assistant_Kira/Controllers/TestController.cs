using Microsoft.AspNetCore.Mvc;

namespace Assistant_Kira.Controllers;

[ApiController]
[Route("api/dev/update")]
public sealed class TestController(ICommandExecutor commandExecutor) : ControllerBase
{
    [HttpGet]
    public async Task<string> Update(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            return "Введите валидную команду";
        }
        return await commandExecutor.ExecuteAsync(command);
    }
}

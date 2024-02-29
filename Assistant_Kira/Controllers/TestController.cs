using Microsoft.AspNetCore.Mvc;

namespace Assistant_Kira.Controllers;

[ApiController]
[Route("api/dev/update")]
public sealed class TestController(ICommandExecutor commandExecutor) : ControllerBase
{
    private readonly ICommandExecutor _commandExecutor = commandExecutor;

    [HttpGet]
    public string Update(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            return "Введите валидную команду";
        }
        return _commandExecutor.Execute(command);
    }
}

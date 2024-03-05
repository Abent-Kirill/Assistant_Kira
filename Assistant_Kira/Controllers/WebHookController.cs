using Assistant_Kira.Models;
using Assistant_Kira.Models.Webhooks.GitLab;

using Microsoft.AspNetCore.Mvc;

using Telegram.Bot;

namespace Assistant_Kira.Controllers;

[Route("webhook")]
[ApiController]
public sealed class WebHookController(KiraBot kiraBot) : ControllerBase
{
    [HttpPost]
    public async Task GitLabWebHook([FromBody] WebHookMessage webHookMessage)
    {
        if (webHookMessage.Stages.Any(x => x.Status.Equals("failed", StringComparison.InvariantCultureIgnoreCase)))
        {
            await kiraBot.SendTextMessageAsync(1548307601, webHookMessage.ToString());
        }
    }
}

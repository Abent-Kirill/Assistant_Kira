
using Assistant_Kira.Services;

namespace Assistant_Kira.Commands
{
    internal class NewsCommand(LentaNewsService lentaNewsService) : ICommand
    {
        public string Name => "Новости";

        public Task<string> ExecuteAsync(IEnumerable<string> args)
        {
            return Task.Run(lentaNewsService.GetCurrentNews);
        }
    }
}

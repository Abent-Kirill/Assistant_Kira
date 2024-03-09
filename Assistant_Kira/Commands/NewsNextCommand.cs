using Assistant_Kira.Services;

namespace Assistant_Kira.Commands
{
    internal class NewsNextCommand(LentaNewsService lentaNewsService) : ICommand
    {
        public string Name => "Вперед";

        public Task<string> ExecuteAsync(IEnumerable<string> args)
        {
            lentaNewsService.NextNews();
            return Task.Run(lentaNewsService.GetCurrentNews);
        }
    }
}

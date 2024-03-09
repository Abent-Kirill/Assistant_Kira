using Assistant_Kira.Services;

namespace Assistant_Kira.Commands
{
    internal class NewsPreviousCommand(LentaNewsService lentaNewsService) : ICommand
    {
        public string Name => "Назад";

        public Task<string> ExecuteAsync(IEnumerable<string> args)
        {
            lentaNewsService.PreviousNews();
            return Task.Run(lentaNewsService.GetCurrentNews);
        }
    }
}

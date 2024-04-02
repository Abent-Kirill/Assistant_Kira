namespace Assistant_Kira.Models;

internal readonly record struct NewsContent(Uri NewsLink, string Title, string Description)
{
    public override readonly string ToString() => $"**{Title}** \n{Description} \n{NewsLink}";
}

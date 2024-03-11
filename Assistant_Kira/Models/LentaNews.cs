namespace Assistant_Kira.Models;

internal struct LentaNews
{
    public LentaNews(Uri newsLink, string title, string description)
    {
        NewsLink = newsLink;
        Title = title;
        Description = description;
    }
    public Uri NewsLink { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }

    public override readonly string ToString() => $"*{Title}* \n{Description} \n{NewsLink}";
}

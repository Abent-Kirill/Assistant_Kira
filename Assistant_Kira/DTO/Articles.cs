using System.Text.Json.Serialization;

namespace Assistant_Kira.DTO;

internal sealed record Articles
{
    [JsonPropertyName("articles")]
    public List<Article> ArticleList { get; init; }

    [JsonConstructor]
    public Articles(List<Article> articleList)
    {
        ArticleList = articleList;
    }
}

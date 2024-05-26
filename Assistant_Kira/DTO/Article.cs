using System.Text.Json.Serialization;

namespace Assistant_Kira.DTO;

internal sealed record Article
{
    [JsonPropertyName("author")]
    public string? Author { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; }

    [JsonPropertyName("url")]
    public Uri Url { get; init; }

    [JsonPropertyName("publishedAt")]
    public DateTimeOffset PublishedAt { get; init; }

    [JsonPropertyName("content")]
    public string Content { get; init; }

    [JsonConstructor]
    public Article(string? author, string title, string description, Uri url,
        DateTimeOffset publishedAt, string content)
    {
        Author = author;
        Title = title;
        Description = description;
        Url = url;
        PublishedAt = publishedAt;
        Content = content;
    }
}

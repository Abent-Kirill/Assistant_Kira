using System.Text.Json.Serialization;

namespace Assistant_Kira.Models.Webhooks.GitLab;

public struct Project
{
	[JsonPropertyName("id")]
	public int Id { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("description")]
	public string Description { get; set; }

	[JsonPropertyName("web_url")]
	public Uri Url { get; set; }

	[JsonPropertyName("avatar_url")]
	public Uri Avatar { get; set; }

	public override readonly string ToString() => $"{Name}\n({Description})\nURL: {Url}";
}

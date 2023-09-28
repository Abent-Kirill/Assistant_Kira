using System.Text.Json.Serialization;

namespace Assistant_Kira.Models.Webhooks.GitLab;

public struct Stage
{
	[JsonPropertyName("id")]
	public int Id { get; set; }

	[JsonPropertyName("stage")]
	public string Name { get; set; }

	[JsonPropertyName("status")]
	public string Status { get; set; }

	[JsonPropertyName("created_at")]
	public DateTime CreatedAt { get; set; }

	[JsonPropertyName("started_at")]
	public DateTime StartedAt { get; set; }

	[JsonPropertyName("finished_at")]
	public DateTime FinishedAt { get; set; }

	[JsonPropertyName("user")]
	public User User { get; set; }

	public override readonly string ToString() => $"{Name}:{Status}";
}

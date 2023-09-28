using System.Text.Json.Serialization;

namespace Assistant_Kira.Models.Webhooks.GitLab;

public struct User
{
	[JsonPropertyName("id")]
	public int Id { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("username")]
	public string UserName { get; set; }
	
	[JsonPropertyName("avatar_url")]
	public Uri Avatar { get; set; }

	[JsonPropertyName("email")]
	public string Email { get; set; }

	public override readonly string ToString() => $"Имя: {Name}\nEmail: {Email}";
}

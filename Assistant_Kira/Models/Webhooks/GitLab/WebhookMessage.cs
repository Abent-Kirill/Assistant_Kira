using System.Text;
using System.Text.Json.Serialization;

namespace Assistant_Kira.Models.Webhooks.GitLab;

public struct WebHookMessage
{
	[JsonPropertyName("user")]
	public User User { get; set; }

	[JsonPropertyName("project")]
	public Project Project { get; set; }

	[JsonPropertyName("builds")]
	public Stage[] Stages { get; set; }

	public override readonly string ToString()
	{
		var strBuilder = new StringBuilder($"Проект: {Project.Name} ({Project.Description})\n\tURL: {Project.Url}\nСтатус этапов:\n");
		foreach (var build in Stages)
		{
			strBuilder.AppendLine($"\t{build}");
		}
		strBuilder.AppendLine($"Разработчик:\n\t{User}");
		return strBuilder.ToString();
	}
}

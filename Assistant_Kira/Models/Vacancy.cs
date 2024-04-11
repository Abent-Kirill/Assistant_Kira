namespace Assistant_Kira.Models;

internal record Vacancy(string Title, string Description, string CompanyName, Uri Link)
{
    public override string ToString() => $"{Title}\n{Description}\n\nКомпания: {CompanyName}\n\nОзнакомиться: {Link}";
}

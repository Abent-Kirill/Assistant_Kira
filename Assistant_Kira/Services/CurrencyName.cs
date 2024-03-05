using System.ComponentModel.DataAnnotations;

namespace Assistant_Kira.Services;

internal enum CurrencyName
{
    [Display(Name = "RUB")]
    RUB,
    [Display(Name = "USD")]
    USD,
    [Display(Name = "KZT")]
    KZT
}

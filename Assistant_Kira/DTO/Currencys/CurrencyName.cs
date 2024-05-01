using System.ComponentModel.DataAnnotations;

namespace Assistant_Kira.DTO.Currencys;

internal enum CurrencyName
{
    [Display(Name = "RUB")]
    RUB,
    [Display(Name = "USD")]
    USD,
    [Display(Name = "KZT")]
    KZT,
    [Display(Name = "EUR")]
    EUR
}

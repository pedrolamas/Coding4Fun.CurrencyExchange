using System;

namespace Coding4Fun.CurrencyExchange.Models
{
    public interface ICurrency
    {
        string Name { get; }

        double CachedExchangeRate { get; set; }

        DateTime CachedExchangeRateUpdatedOn { get; set; }
    }
}
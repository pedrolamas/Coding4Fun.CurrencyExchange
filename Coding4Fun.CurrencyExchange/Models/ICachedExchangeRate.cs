using System;

namespace Coding4Fun.CurrencyExchange.Model
{
    public interface ICachedExchangeRate
    {
        double ExchangeRate { get; }

        DateTime LastUpdate { get; }
    }
}
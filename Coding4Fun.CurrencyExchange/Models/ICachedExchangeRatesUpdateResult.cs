using System;

namespace Coding4Fun.CurrencyExchange.Models
{
    public interface ICachedExchangeRatesUpdateResult
    {
        Exception Error { get; }

        object State { get; }
    }
}
using System;

namespace Coding4Fun.CurrencyExchange.Models
{
    public interface ICurrencyExchangeResult
    {
        Exception Error { get; }

        ICurrency ExchangedCurrency { get; }

        double ExchangedAmount { get; }

        DateTime Timestamp { get; }

        object State { get; }
    }
}
using System;

namespace Coding4Fun.CurrencyExchange.Model
{
    public interface ICurrencyExchangeResult
    {
        Exception Error { get; }

        string ExchangedCurrency { get; }

        double ExchangedAmount { get; }
    }
}
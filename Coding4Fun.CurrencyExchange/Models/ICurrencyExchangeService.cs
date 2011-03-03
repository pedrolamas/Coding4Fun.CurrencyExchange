using System;
using System.Collections.Generic;

namespace Coding4Fun.CurrencyExchange.Models
{
    public interface ICurrencyExchangeService
    {
        ICurrency[] Currencies { get; }

        ICurrency BaseCurrency { get; }

        void ExchangeCurrency(double amount, ICurrency fromCurrency, ICurrency toCurrency, bool useCachedExchangeRates, Action<ICurrencyExchangeResult> callback, object state);

        void UpdateCachedExchangeRates(IEnumerable<ICurrency> currencies, Action<CachedExchangeRatesUpdateResult> callback, object state);
    }
}
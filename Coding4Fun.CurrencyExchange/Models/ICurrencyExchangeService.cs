using System;
using System.Collections.Generic;

namespace Coding4Fun.CurrencyExchange.Model
{
    public interface ICurrencyExchangeService
    {
        ICurrency[] Currencies { get; }

        Dictionary<ICurrency, ICachedExchangeRate> CachedExchangeRates { get; set; }

        void ExchangeCurrency(double amount, ICurrency fromCurrency, ICurrency toCurrency, bool useCachedExchangeRates, Action<ICurrencyExchangeResult> callback, object state);

        void UpdateCachedExchangeRates(Action<object> callback, object state);
    }
}
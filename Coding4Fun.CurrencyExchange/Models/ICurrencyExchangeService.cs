using System;

namespace Coding4Fun.CurrencyExchange.Model
{
    public interface ICurrencyExchangeService
    {
        ICurrency[] Currencies { get; }

        void ExchangeCurrency(double amount, ICurrency fromCurrency, ICurrency toCurrency, Action<ICurrencyExchangeResult> callback);
    }
}
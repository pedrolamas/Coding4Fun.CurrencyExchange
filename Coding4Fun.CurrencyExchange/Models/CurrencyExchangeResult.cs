using System;

namespace Coding4Fun.CurrencyExchange.Model
{
    public class CurrencyExchangeResult : ICurrencyExchangeResult
    {
        #region Properties

        public Exception Error { get; protected set; }

        public string ExchangedCurrency { get; protected set; }

        public double ExchangedAmount { get; protected set; }

        #endregion

        public CurrencyExchangeResult(Exception error)
        {
            Error = error;
        }

        public CurrencyExchangeResult(string exchangedCurrency, double exchangedAmount)
        {
            ExchangedCurrency = exchangedCurrency;
            ExchangedAmount = exchangedAmount;
        }
    }
}
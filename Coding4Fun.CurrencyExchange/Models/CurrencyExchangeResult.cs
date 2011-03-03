using System;

namespace Coding4Fun.CurrencyExchange.Models
{
    public class CurrencyExchangeResult : ICurrencyExchangeResult
    {
        #region Properties

        public Exception Error { get; protected set; }

        public ICurrency ExchangedCurrency { get; protected set; }

        public double ExchangedAmount { get; protected set; }

        public DateTime Timestamp { get; protected set; }

        public object State { get; protected set; }

        #endregion

        public CurrencyExchangeResult(Exception error, object state)
        {
            Error = error;
            State = state;
        }

        public CurrencyExchangeResult(ICurrency exchangedCurrency, double exchangedAmount, DateTime timestamp, object state)
        {
            ExchangedCurrency = exchangedCurrency;
            ExchangedAmount = exchangedAmount;
            Timestamp = timestamp;
            State = state;
        }
    }
}
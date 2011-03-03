using System;

namespace Coding4Fun.CurrencyExchange.Models
{
    public class CachedExchangeRatesUpdateResult : ICachedExchangeRatesUpdateResult
    {
        #region Properties

        public Exception Error { get; protected set; }

        public object State { get; protected set; }

        #endregion

        public CachedExchangeRatesUpdateResult(Exception error, object state)
        {
            Error = error;
            State = state;
        }

        public CachedExchangeRatesUpdateResult(object state)
        {
            State = state;
        }
    }
}
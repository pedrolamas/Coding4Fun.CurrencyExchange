using System;

namespace Coding4Fun.CurrencyExchange.Model
{
    public class CachedExchangeRate : ICachedExchangeRate
    {
        #region Properties

        public double ExchangeRate { get; private set; }

        public DateTime LastUpdate { get; private set; }

        #endregion

        public CachedExchangeRate()
        {
        }

        public CachedExchangeRate(double exchangeRate, DateTime lastUpdate)
        {
            ExchangeRate = exchangeRate;
            LastUpdate = lastUpdate;
        }
    }
}
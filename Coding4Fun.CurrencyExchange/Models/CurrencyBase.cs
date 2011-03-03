using System;

namespace Coding4Fun.CurrencyExchange.Models
{
    public abstract class CurrencyBase : ICurrency
    {
        #region Properties

        public string Name { get; protected set; }

        public double CachedExchangeRate { get; set; }

        public DateTime CachedExchangeRateUpdatedOn { get; set; }

        #endregion
    }
}
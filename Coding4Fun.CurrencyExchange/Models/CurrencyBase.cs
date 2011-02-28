using System;

namespace Coding4Fun.CurrencyExchange.Model
{
    public abstract class CurrencyBase : ICurrency
    {
        #region Properties

        public string Name { get; protected set; }

        double ExchangeRate { get; protected set; }

        DateTime ExchangeRateLastUpdate { get; protected set; }

        #endregion

        public CurrencyBase(string name)
        {
            Name = name;
        }
    }
}
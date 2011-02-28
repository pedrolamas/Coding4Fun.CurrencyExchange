using System;
using System.Linq;
using Coding4Fun.CurrencyExchange.Model;

namespace Coding4Fun.CurrencyExchange.ViewModels
{
    public class FavoriteCurrenciesViewModel
    {
        #region Properties

        public ICurrency[] Currencies
        {
            get
            {
                return _mainViewModel.Currencies;
            }
        }

        public ICurrency[] FavoriteCurrencies
        {
            get
            {
                return _mainViewModel.CachedExchangeRates
                    .Select(x => x.Key)
                    .ToArray();
            }
            set
            {
                var oldFavoriteCurrencies = FavoriteCurrencies;

                foreach (var currency in oldFavoriteCurrencies)
                {
                    if (!value.Contains(currency))
                        _mainViewModel.CachedExchangeRates.Remove(currency);
                }

                foreach (var currency in value)
                {
                    if (_mainViewModel.CachedExchangeRates.ContainsKey(currency))
                        continue;

                    _mainViewModel.CachedExchangeRates.Add(currency, new CachedExchangeRate(0, DateTime.MinValue));
                }
            }
        }

        #endregion

        private MainViewModel _mainViewModel;

        public FavoriteCurrenciesViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}
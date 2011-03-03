using System.Linq;
using Coding4Fun.CurrencyExchange.Models;

namespace Coding4Fun.CurrencyExchange.ViewModels
{
    public class FavoriteCurrenciesViewModel
    {
        private MainViewModel _mainViewModel;

        #region Properties

        public ICurrency[] Currencies
        {
            get
            {
                return _mainViewModel.Currencies
                    .Where(x => x.CachedExchangeRate != 1.0)
                    .ToArray();
            }
        }

        public ICurrency[] FavoriteCurrencies
        {
            get
            {
                return _mainViewModel.FavoriteCurrencies;
            }
            set
            {
                _mainViewModel.FavoriteCurrencies = value;
            }
        }

        #endregion

        public FavoriteCurrenciesViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public void Save()
        {
            _mainViewModel.Save();
        }
    }
}
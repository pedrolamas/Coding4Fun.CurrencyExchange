using System.Linq;
using Coding4Fun.CurrencyExchange.Models;
using Coding4Fun.CurrencyExchange.ViewModels;
using Microsoft.Phone.Controls;

namespace Coding4Fun.CurrencyExchange
{
    public partial class FavoriteCurrenciesPage : PhoneApplicationPage
    {
        public FavoriteCurrenciesPage()
        {
            InitializeComponent();

            var viewModel = new FavoriteCurrenciesViewModel(MainViewModel.Instance);

            this.DataContext = viewModel;

            foreach (var currency in viewModel.FavoriteCurrencies)
                FavoriteCurrencies.SelectedItems.Add(currency);
        }

        private void OkApplicationBarIconButton_Click(object sender, System.EventArgs e)
        {
            var viewModel = this.DataContext as FavoriteCurrenciesViewModel;

            if (viewModel != null)
            {
                viewModel.FavoriteCurrencies = FavoriteCurrencies.SelectedItems
                    .Cast<ICurrency>()
                    .ToArray();

                viewModel.Save();
            }

            NavigationService.GoBack();
        }

        private void CancelApplicationBarIconButton_Click(object sender, System.EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
using System;
using System.Windows;
using System.Windows.Controls;
using Coding4Fun.CurrencyExchange.ViewModels;
using Coding4Fun.Phone.Site.Controls;
using Microsoft.Phone.Controls;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace Coding4Fun.CurrencyExchange
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            this.DataContext = MainViewModel.Instance;
        }

        private void PhoneApplicationPage_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                MessageBox.Show(e.Error.Exception.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void ExchangeIconButton_Click(object sender, EventArgs e)
        {
            var viewModel = DataContext as MainViewModel;

            if (viewModel == null)
                return;

            Focus();

            Dispatcher.BeginInvoke(() =>
            {
                viewModel.Save();

                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    MessageBox.Show("No network connection found!", "Error", MessageBoxButton.OK);

                    return;
                }

                viewModel.ExchangeCurrency();
            });
        }

        private void EditFavoriteCurrenciesMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/FavoriteCurrenciesPage.xaml", UriKind.Relative));
        }

        private void UpdateExchangeRatesMenuItem_Click(object sender, EventArgs e)
        {
            var viewModel = DataContext as MainViewModel;

            if (viewModel == null)
                return;

            Dispatcher.BeginInvoke(() =>
            {
                viewModel.UpdateCachedExchangeRates();
            });
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            var about = new Coding4FunAboutPrompt();

            about.Show("Pedro Lamas", "pedrolamas", "pedrolamas@gmail.com", "http://www.pedrolamas.com");
        }
    }
}
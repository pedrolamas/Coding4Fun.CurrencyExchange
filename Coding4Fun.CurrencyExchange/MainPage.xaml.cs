using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Coding4Fun.CurrencyExchange.Helpers;
using Coding4Fun.CurrencyExchange.Model;
using Coding4Fun.CurrencyExchange.ViewModels;
using Coding4Fun.Phone.Site.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace Coding4Fun.CurrencyExchange
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            this.DataContext = MainViewModel.Instance;
            MainViewModel.Instance.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Instance_PropertyChanged);
        }

        void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "ExchangedAmount")
                progressOverlay.Hide();
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
                                        progressOverlay.Show();
										viewModel.Save();

										if (!NetworkInterface.GetIsNetworkAvailable())
										{
											MessageBox.Show("No network connection found!", "Error", MessageBoxButton.OK);
											return;
										}

										viewModel.ExchangeCurrency();
                                        
									});
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            var about = new Coding4FunAboutPrompt();
            about.Show("Pedro Lamas", "pedrolamas", "pedrolamas@gmail.com", "http://www.pedrolamas.com");
        }
    }
}
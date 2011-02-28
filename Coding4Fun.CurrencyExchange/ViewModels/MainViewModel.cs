using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using Coding4Fun.CurrencyExchange.Helpers;
using Coding4Fun.CurrencyExchange.Model;

namespace Coding4Fun.CurrencyExchange.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private const string SettingFileName = "mainviewmodel.dat";

        private ICurrencyExchangeService _currencyExchangeService;
        private bool _busy = false;
        private double _amount;
        private ICurrency _fromCurrency;
        private ICurrency _toCurrency;
        private ICurrencyExchangeResult _result;

        #region Properties

        [IgnoreDataMember]
        public static MainViewModel Instance { get; protected set; }

        [IgnoreDataMember]
        public ICurrencyExchangeService CurrencyExchangeService
        {
            get
            {
                return _currencyExchangeService;
            }
            set
            {
                if (_currencyExchangeService == value)
                    return;

                _currencyExchangeService = value;

                _fromCurrency = Currencies.FirstOrDefault(x => x.Name == "US Dollar") ?? Currencies[0];
                _toCurrency = Currencies.FirstOrDefault(x => x.Name == "Euro") ?? Currencies[1];

                RaisePropertyChanged("CurrencyExchangeService");
                RaisePropertyChanged("Currencies");
            }
        }

        [IgnoreDataMember]
        public ICurrency[] Currencies
        {
            get
            {
                return _currencyExchangeService.Currencies;
            }
        }

        [IgnoreDataMember]
        public Dictionary<ICurrency, ICachedExchangeRate> CachedExchangeRates
        {
            get
            {
                return _currencyExchangeService.CachedExchangeRates;
            }
        }

        [DataMember]
        public string Amount
        {
            get
            {
                return _amount.ToString("0.00");
            }
            set
            {
                double amount;

                if (double.TryParse(value, out amount))
                {
                    if (_amount == amount)
                        return;

                    _amount = amount;

                    RaisePropertyChanged("Amount");
                }
                else
                    throw new Exception("Please enter a valid Amount");
            }
        }

        [IgnoreDataMember]
        public ICurrency FromCurrency
        {
            get
            {
                return _fromCurrency;
            }
            set
            {
                if (_fromCurrency == value)
                    return;

                _fromCurrency = value;

                RaisePropertyChanged("FromCurrency");
            }
        }

        [IgnoreDataMember]
        public ICurrency ToCurrency
        {
            get
            {
                return _toCurrency;
            }
            set
            {
                if (_toCurrency == value)
                    return;

                _toCurrency = value;

                RaisePropertyChanged("ToCurrency");
            }
        }

        [IgnoreDataMember]
        public ICurrencyExchangeResult Result
        {
            get
            {
                return _result;
            }
            protected set
            {
                if (_result == value)
                    return;

                _result = value;

                RaisePropertyChanged("Result");
                RaisePropertyChanged("ExchangedCurrency");
                RaisePropertyChanged("ExchangedAmount");
                RaisePropertyChanged("ExchangedTimeStamp");
            }
        }

        [IgnoreDataMember]
        public string ExchangedCurrency
        {
            get
            {
                if (_result == null)
                    return string.Empty;

                return _result.ExchangedCurrency.Name;
            }
        }

        [IgnoreDataMember]
        public string ExchangedAmount
        {
            get
            {
                if (_result == null)
                    return string.Empty;

                return _result.ExchangedAmount.ToString("N2");
            }
        }

        [IgnoreDataMember]
        public string ExchangedTimeStamp
        {
            get
            {
                if (_result == null)
                    return string.Empty;

                return string.Format("Data retrieved on {0}, at {1}",
                    _result.Timestamp.ToLongDateString(),
                    _result.Timestamp.ToShortTimeString());
            }
        }

        [DataMember]
        public int FromCurrencyIndex
        {
            get
            {
                return Array.IndexOf(Currencies, FromCurrency);
            }
            set
            {
                FromCurrency = Currencies[value];
            }
        }

        [DataMember]
        public int ToCurrencyIndex
        {
            get
            {
                return Array.IndexOf(Currencies, ToCurrency);
            }
            set
            {
                ToCurrency = Currencies[value];
            }
        }

        [IgnoreDataMember]
        public bool Busy
        {
            get
            {
                return _busy;
            }
            set
            {
                if (_busy == value)
                    return;

                _busy = value;

                RaisePropertyChanged("Busy");
                RaisePropertyChanged("BusyVisibility");
            }
        }

        [IgnoreDataMember]
        public Visibility BusyVisibility
        {
            get
            {
                return Busy ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion

        public MainViewModel()
        {
            CurrencyExchangeService = new BingCurrencyExchangeService();
            Amount = "100";
        }

        static MainViewModel()
        {
            Instance = Load();
        }

        public void ExchangeCurrency()
        {
            if (Busy)
                return;

            Busy = true;

            _currencyExchangeService.ExchangeCurrency(_amount, _fromCurrency, _toCurrency, true, CurrencyExchanged, null);
        }

        public void UpdateCachedExchangeRates()
        {
            if (Busy)
                return;

            Busy = true;

            _currencyExchangeService.UpdateCachedExchangeRates(ExchangeRatesUpdated, null);
        }

        public static MainViewModel Load()
        {
            return StorageHelper.LoadContract<MainViewModel>(SettingFileName, true);
        }

        public void Save()
        {
            StorageHelper.SaveContract(SettingFileName, this, true);
        }

        private void CurrencyExchanged(ICurrencyExchangeResult result)
        {
            InvokeOnUiThread(() =>
            {
                Result = result;

                Busy = false;
            });
        }

        private void ExchangeRatesUpdated(object state)
        {
            InvokeOnUiThread(() =>
            {
                Busy = false;
            });
        }

        private void InvokeOnUiThread(Action action)
        {
            var dispatcher = System.Windows.Deployment.Current.Dispatcher;

            if (dispatcher.CheckAccess())
                action();
            else
                dispatcher.BeginInvoke(action);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
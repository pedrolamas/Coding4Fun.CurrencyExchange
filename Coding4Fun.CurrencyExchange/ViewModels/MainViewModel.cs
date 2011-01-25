using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

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
            }
        }

        [IgnoreDataMember]
        public string ExchangedCurrency
        {
            get
            {
                if (_result == null)
                    return string.Empty;

                return _result.ExchangedCurrency;
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
            if (_busy)
                return;

            _busy = true;

            _currencyExchangeService.ExchangeCurrency(_amount, _fromCurrency, _toCurrency, CurrencyExchanged);
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
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                Result = result;

                _busy = false;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
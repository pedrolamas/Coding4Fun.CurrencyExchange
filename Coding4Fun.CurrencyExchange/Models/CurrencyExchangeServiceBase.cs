using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Coding4Fun.CurrencyExchange.Model
{
    public abstract class CurrencyExchangeServiceBase : ICurrencyExchangeService
    {
        private ICurrency _baseCurrency;

        #region Properties

        public abstract ICurrency[] Currencies { get; }

        public Dictionary<ICurrency, ICachedExchangeRate> CachedExchangeRates { get; set; }

        #endregion

        public CurrencyExchangeServiceBase()
        {
            CachedExchangeRates = new Dictionary<ICurrency, ICachedExchangeRate>();

            _baseCurrency = Currencies.FirstOrDefault(x => x.Name == "US Dollar") ??
                Currencies.FirstOrDefault(x => x.Name == "Euro") ??
                Currencies[0];
        }

        protected abstract string CreateRequestUrl(double amount, ICurrency fromCurrency, ICurrency toCurrency);

        protected abstract double GetResultFromResponseContent(string responseContent);

        public void ExchangeCurrency(double amount, ICurrency fromCurrency, ICurrency toCurrency, bool useCachedExchangeRates, Action<ICurrencyExchangeResult> callback, object state)
        {
            if (useCachedExchangeRates)
            {
                var fromExchangeRate = 0.0;
                var toExchangeRate = 0.0;
                var timestamp = DateTime.Now;

                if (fromCurrency == _baseCurrency)
                    fromExchangeRate = 1;
                else if (CachedExchangeRates.ContainsKey(fromCurrency))
                {
                    var fromCachedExchangeRate = CachedExchangeRates[fromCurrency];

                    fromExchangeRate = fromCachedExchangeRate.ExchangeRate;

                    if (timestamp > fromCachedExchangeRate.LastUpdate)
                        timestamp = fromCachedExchangeRate.LastUpdate;
                }

                if (toCurrency == _baseCurrency)
                    toExchangeRate = 1;
                else if (CachedExchangeRates.ContainsKey(toCurrency))
                {
                    var toCachedExchangeRate = CachedExchangeRates[toCurrency];

                    toExchangeRate = toCachedExchangeRate.ExchangeRate;

                    if (timestamp > toCachedExchangeRate.LastUpdate)
                        timestamp = toCachedExchangeRate.LastUpdate;
                }

                if (fromExchangeRate > 0 && toExchangeRate > 0)
                {
                    var exchangedAmount = amount / fromExchangeRate * toExchangeRate;

                    callback(new CurrencyExchangeResult(toCurrency, exchangedAmount, timestamp, state));

                    return;
                }
            }

            var url = CreateRequestUrl(amount, fromCurrency, toCurrency);

            var request = HttpWebRequest.Create(url);

            request.BeginGetResponse(ar =>
            {
                try
                {
                    var response = (HttpWebResponse)request.EndGetResponse(ar);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string responseContent;

                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            responseContent = streamReader.ReadToEnd();
                        }

                        var exchangedAmount = GetResultFromResponseContent(responseContent);

                        callback(new CurrencyExchangeResult(toCurrency, exchangedAmount, DateTime.Now, ar.AsyncState));
                    }
                    else
                    {
                        throw new Exception(string.Format("Http Error: ({0}) {1}",
                            response.StatusCode,
                            response.StatusDescription));
                    }
                }
                catch (Exception ex)
                {
                    callback(new CurrencyExchangeResult(ex, ar.AsyncState));
                }
            }, state);
        }

        public void UpdateCachedExchangeRates(Action<object> callback, object state)
        {
            UpdateNextCachedExchangeRate(new UpdateCachedExchangeRatesState(callback, state, CachedExchangeRates.Keys));
        }

        private void UpdateNextCachedExchangeRate(UpdateCachedExchangeRatesState updateCachedExchangeRatesState)
        {
            if (updateCachedExchangeRatesState.CurrenciesStack.Count == 0)
                updateCachedExchangeRatesState.Callback(updateCachedExchangeRatesState.UserState);
            else
            {
                var currency = updateCachedExchangeRatesState.CurrenciesStack.Pop();

                ExchangeCurrency(1, _baseCurrency, currency, false, UpdateCachedExchangeRate, updateCachedExchangeRatesState);
            }
        }

        private void UpdateCachedExchangeRate(ICurrencyExchangeResult result)
        {
            var updateCachedExchangeRatesState = (UpdateCachedExchangeRatesState)result.State;

            CachedExchangeRates[result.ExchangedCurrency] = new CachedExchangeRate(result.ExchangedAmount, result.Timestamp);

            UpdateNextCachedExchangeRate(updateCachedExchangeRatesState);
        }

        private class UpdateCachedExchangeRatesState
        {
            #region Properties

            public Action<object> Callback { get; private set; }

            public object UserState { get; set; }

            public Stack<ICurrency> CurrenciesStack { get; set; }

            #endregion

            public UpdateCachedExchangeRatesState(Action<object> callback, object userState, IEnumerable<ICurrency> currencies)
            {
                Callback = callback;
                UserState = userState;
                CurrenciesStack = new Stack<ICurrency>(currencies);
            }
        }
    }
}
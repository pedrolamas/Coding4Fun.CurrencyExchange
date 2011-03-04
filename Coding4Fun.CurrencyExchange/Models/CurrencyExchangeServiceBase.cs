using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Coding4Fun.CurrencyExchange.Models
{
    public abstract class CurrencyExchangeServiceBase : ICurrencyExchangeService
    {
        #region Properties

        public abstract ICurrency[] Currencies { get; }

        public abstract ICurrency BaseCurrency { get; protected set; }

        #endregion

        protected abstract string CreateRequestUrl(double amount, ICurrency fromCurrency, ICurrency toCurrency);

        protected abstract double GetResultFromResponseContent(string responseContent);

        public void ExchangeCurrency(double amount, ICurrency fromCurrency, ICurrency toCurrency, bool useCachedExchangeRates, Action<ICurrencyExchangeResult> callback, object state)
        {
            if (useCachedExchangeRates)
            {
                var fromExchangeRate = fromCurrency.CachedExchangeRate;
                var toExchangeRate = toCurrency.CachedExchangeRate;
                var timestamp = DateTime.Now;

                if (fromCurrency == BaseCurrency)
                    fromExchangeRate = 1.0;
                else
                {
                    if (timestamp > fromCurrency.CachedExchangeRateUpdatedOn)
                        timestamp = fromCurrency.CachedExchangeRateUpdatedOn;
                }

                if (toCurrency == BaseCurrency)
                    toExchangeRate = 1.0;
                else
                {
                    if (timestamp > toCurrency.CachedExchangeRateUpdatedOn)
                        timestamp = toCurrency.CachedExchangeRateUpdatedOn;
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

        public void UpdateCachedExchangeRates(Action<CachedExchangeRatesUpdateResult> callback, object state)
        {
            UpdateNextCachedExchangeRate(new UpdateCachedExchangeRatesState(callback, state, Currencies.Cast<ICurrency>().GetEnumerator()));
        }

        private void UpdateNextCachedExchangeRate(UpdateCachedExchangeRatesState updateCachedExchangeRatesState)
        {
            var currenciesEnumerator = updateCachedExchangeRatesState.CurrenciesEnumerator;

            if (!currenciesEnumerator.MoveNext())
            {
                currenciesEnumerator.Dispose();

                updateCachedExchangeRatesState.Callback(new CachedExchangeRatesUpdateResult(updateCachedExchangeRatesState.UserState));
            }
            else
            {
                var currency = currenciesEnumerator.Current;

                ExchangeCurrency(1, BaseCurrency, currency, false, UpdateCachedExchangeRate, updateCachedExchangeRatesState);
            }
        }

        private void UpdateCachedExchangeRate(ICurrencyExchangeResult result)
        {
            var updateCachedExchangeRatesState = (UpdateCachedExchangeRatesState)result.State;

            if (result.Error != null)
            {
                updateCachedExchangeRatesState.CurrenciesEnumerator.Dispose();

                updateCachedExchangeRatesState.Callback(new CachedExchangeRatesUpdateResult(result.Error, updateCachedExchangeRatesState.UserState));
            }

            result.ExchangedCurrency.CachedExchangeRate = result.ExchangedAmount;
            result.ExchangedCurrency.CachedExchangeRateUpdatedOn = result.Timestamp;

            UpdateNextCachedExchangeRate(updateCachedExchangeRatesState);
        }

        #region Auxiliary Classes

        private class UpdateCachedExchangeRatesState
        {
            #region Properties

            public Action<CachedExchangeRatesUpdateResult> Callback { get; private set; }

            public object UserState { get; set; }

            public IEnumerator<ICurrency> CurrenciesEnumerator { get; set; }

            #endregion

            public UpdateCachedExchangeRatesState(Action<CachedExchangeRatesUpdateResult> callback, object userState, IEnumerator<ICurrency> currenciesEnumerator)
            {
                Callback = callback;
                UserState = userState;
                CurrenciesEnumerator = currenciesEnumerator;
            }
        }

        #endregion
    }
}
using System;
using System.IO;
using System.Net;

namespace Coding4Fun.CurrencyExchange.Model
{
    public abstract class CurrencyExchangeServiceBase : ICurrencyExchangeService
    {
        public abstract ICurrency[] Currencies { get; }

        protected abstract string CreateRequestUrl(double amount, ICurrency fromCurrency, ICurrency toCurrency);

        protected abstract double GetResultFromResponseContent(string responseContent);

        public void ExchangeCurrency(double amount, ICurrency fromCurrency, ICurrency toCurrency, Action<ICurrencyExchangeResult> callback)
        {
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

                        var exchangedCurrency = GetResultFromResponseContent(responseContent);

                        callback(new CurrencyExchangeResult(toCurrency.Name, exchangedCurrency));
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
                    callback(new CurrencyExchangeResult(ex));
                }
            }, null);
        }
    }
}
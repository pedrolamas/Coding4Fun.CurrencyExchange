using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Coding4Fun.CurrencyExchange.Models
{
    public class MsnMoneyV2CurrencyExchangeService : ICurrencyExchangeService
    {
        private const string MsnMoneyUrl = "http://investing.money.msn.com/investments/exchange-rates";
        private const string MsnMoneyPostData = "regiondropdown=1&comparecurrencydropdown=USD";

        #region Static Globals

        private static Regex _resultRegex = new Regex(@"<tr><td.*?(?<currency>[^<>]+)</a></td>.*?<td class=""currratesper"">(?<value>[0-9.,]+)</td></tr>");

        private static ICurrency[] _currencies = new ICurrency[] { 
            new MsnMoneyCurrency("Algerian Dinar", 48),
            new MsnMoneyCurrency("Argentine Peso", 10),
            new MsnMoneyCurrency("Australian Dollar", 8),
            new MsnMoneyCurrency("Bahraini Dinar", 57),
            new MsnMoneyCurrency("Bolivian Boliviano", 13),
            new MsnMoneyCurrency("Brazilian Real", 14),
            new MsnMoneyCurrency("British Pound", 3),
            new MsnMoneyCurrency("Botswana Pula", 52),
            new MsnMoneyCurrency("Canadian Dollar", 2),
            new MsnMoneyCurrency("Chilean Peso", 16),
            new MsnMoneyCurrency("Chinese Yuan", 17),
            new MsnMoneyCurrency("Columbian Peso", 18),
            new MsnMoneyCurrency("Czech Koruna", 19),
            new MsnMoneyCurrency("Danish Krone", 20),
            new MsnMoneyCurrency("Egyptian Pound", 92),
            new MsnMoneyCurrency("Euro", 9),
            new MsnMoneyCurrency("Ghanaian Cedi", 53),
            new MsnMoneyCurrency("Guatemalan Quetzal", 45),
            new MsnMoneyCurrency("Hong Kong Dollar", 24),
            new MsnMoneyCurrency("Hungarian Forint", 25),
            new MsnMoneyCurrency("Israeli New Shekel", 50),
            new MsnMoneyCurrency("Indian Rupee", 59),
            new MsnMoneyCurrency("Indonesian Rupiah", 26),
            new MsnMoneyCurrency("Japanese Yen", 7),
            new MsnMoneyCurrency("Jordanian Dinar", 60),
            new MsnMoneyCurrency("Kenyan Schilling", 54),
            new MsnMoneyCurrency("Korean Won", 28),
            new MsnMoneyCurrency("Kuwaiti Dinar", 61),
            new MsnMoneyCurrency("Malaysian Ringgit", 51),
            new MsnMoneyCurrency("Mexican Peso", 30),
            new MsnMoneyCurrency("Moroccan Dirham", 46),
            new MsnMoneyCurrency("Namibian Dollar", 55),
            new MsnMoneyCurrency("New Zealand Dollar", 32),
            new MsnMoneyCurrency("Norwegian Krone", 33),
            new MsnMoneyCurrency("Oman Riyal", 63),
            new MsnMoneyCurrency("Pakistani Rupee", 64),
            new MsnMoneyCurrency("Peruvian Nuevo Sol", 34),
            new MsnMoneyCurrency("Philippine Peso", 65),
            new MsnMoneyCurrency("Qatari Riyal", 66),
            new MsnMoneyCurrency("Russian Ruble", 37),
            new MsnMoneyCurrency("Saudi Arabian Riyal", 67),
            new MsnMoneyCurrency("Singapore Dollar", 40),
            new MsnMoneyCurrency("South African Rand", 44),
            new MsnMoneyCurrency("Swedish Krona", 39),
            new MsnMoneyCurrency("Swiss Franc", 15),
            new MsnMoneyCurrency("Taiwan Dollar", 42),
            new MsnMoneyCurrency("Tanzanian Schilling", 68),
            new MsnMoneyCurrency("Thai Baht", 41),
            new MsnMoneyCurrency("Tunisian Dinar", 47),
            new MsnMoneyCurrency("Turkish Lira", 49),
            new MsnMoneyCurrency("UAE Dirham", 69),
            new MsnMoneyCurrency("US Dollar", 1),
            new MsnMoneyCurrency("Venezuelan Bolivar", 43),
            new MsnMoneyCurrency("Vietnamese Dong", 70),
            new MsnMoneyCurrency("Zimbabwean Dollar", 56),
        };

        #endregion

        #region Properties

        public ICurrency[] Currencies
        {
            get
            {
                return _currencies;
            }
        }

        public ICurrency BaseCurrency { get; protected set; }

        #endregion

        public MsnMoneyV2CurrencyExchangeService()
        {
            BaseCurrency = Currencies.First(x => x.Name == "US Dollar");
        }

        public void ExchangeCurrency(double amount, ICurrency fromCurrency, ICurrency toCurrency, bool useCachedExchangeRates, Action<ICurrencyExchangeResult> callback, object state)
        {
            if (useCachedExchangeRates)
            {
                try
                {
                    ExchangeCurrency(amount, fromCurrency, toCurrency, callback, state);

                    return;
                }
                catch
                {
                }
            }

            UpdateCachedExchangeRates(result =>
            {
                if (result.Error != null)
                {
                    callback(new CurrencyExchangeResult(result.Error, state));

                    return;
                }

                try
                {
                    ExchangeCurrency(amount, fromCurrency, toCurrency, callback, state);
                }
                catch (Exception ex)
                {
                    callback(new CurrencyExchangeResult(ex, state));
                }
            }, state);
        }

        private void ExchangeCurrency(double amount, ICurrency fromCurrency, ICurrency toCurrency, Action<ICurrencyExchangeResult> callback, object state)
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
            }
            else
                throw new Exception("Conversion not returned!");
        }

        public void UpdateCachedExchangeRates(Action<CachedExchangeRatesUpdateResult> callback, object state)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(MsnMoneyUrl);
            var data = Encoding.UTF8.GetBytes(MsnMoneyPostData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            request.BeginGetRequestStream(ar2 =>
            {
                var requestStream = request.EndGetRequestStream(ar2);

                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

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

                            foreach (var match in _resultRegex.Matches(responseContent).Cast<Match>())
                            {
                                var currencyName = match.Groups["currency"].Value.Trim();

                                var currency = Currencies.FirstOrDefault(x => string.Compare(x.Name, currencyName, StringComparison.InvariantCultureIgnoreCase) == 0);

                                if (currency != null)
                                {
                                    currency.CachedExchangeRate = double.Parse(match.Groups["value"].Value, CultureInfo.InvariantCulture);
                                    currency.CachedExchangeRateUpdatedOn = DateTime.Now;
                                }
                            }

                            callback(new CachedExchangeRatesUpdateResult(ar.AsyncState));
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
                        callback(new CachedExchangeRatesUpdateResult(ex, ar.AsyncState));
                    }
                }, state);

            }, null);
        }
    }
}
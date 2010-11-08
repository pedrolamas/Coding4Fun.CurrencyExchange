using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Coding4Fun.CurrencyExchange.Model
{
    public class BingCurrencyExchangeService : CurrencyExchangeServiceBase
    {
        #region Static Globals

        private static Regex _resultRegex = new Regex("<span class=\"sc_bigLine\">.*? = (?<value>[0-9.,]+).*?</span>");

        private static ICurrency[] _currencies = new ICurrency[] { 
            new BingCurrency("Algerian Dinar"),
            new BingCurrency("Argentine Peso"),
            new BingCurrency("Australian Dollar"),
            new BingCurrency("Baharaini Dinar"),
            new BingCurrency("Bolivian Boliviano"),
            new BingCurrency("Brazilian Real"),
            new BingCurrency("British Pound"),
            new BingCurrency("Botswana Pula"),
            new BingCurrency("Canadian Dollar"),
            new BingCurrency("Chilean Peso"),
            new BingCurrency("Chinese Yuan"),
            new BingCurrency("Colombian Peso"),
            new BingCurrency("Czech Koruna"),
            new BingCurrency("Danish Krone"),
            new BingCurrency("Egyptian Pound"),
            new BingCurrency("Euro"),
            new BingCurrency("Ghanaian Cedi"),
            new BingCurrency("Guatemalan Quetzal"),
            new BingCurrency("Hong Kong Dollar"),
            new BingCurrency("Hungarian Forint"),
            new BingCurrency("new Israeli shekel"),
            new BingCurrency("Indian Rupee"),
            new BingCurrency("Indonesian Rupiah"),
            new BingCurrency("Japanese Yen"),
            new BingCurrency("Jordanian Dinar"),
            new BingCurrency("Kenyan Shilling"),
            new BingCurrency("Korean Won"),
            new BingCurrency("Kuwaiti Dinar"),
            new BingCurrency("Malaysian Ringgit"),
            new BingCurrency("Mexican Peso"),
            new BingCurrency("Moroccan Dirham"),
            new BingCurrency("Namibian Dollar"),
            new BingCurrency("New Zealand Dollar"),
            new BingCurrency("Norwegian Krone"),
            new BingCurrency("Omani Rial"),
            new BingCurrency("Pakistan Rupee"),
            new BingCurrency("Peruvian Nuevo Sol"),
            new BingCurrency("Philippine Peso"),
            new BingCurrency("Qatari Rial"),
            new BingCurrency("Russian Rouble"),
            new BingCurrency("Saudi Riyal"),
            new BingCurrency("Singapore Dollar"),
            new BingCurrency("South African Rand"),
            new BingCurrency("Swedish Krona"),
            new BingCurrency("Swiss Franc"),
            new BingCurrency("Taiwan Dollar"),
            new BingCurrency("Tanzanian Shilling"),
            new BingCurrency("Thai Baht"),
            new BingCurrency("Tunisian Dinar"),
            new BingCurrency("Turkish Lira"),
            new BingCurrency("Emirati Dirham"),
            new BingCurrency("US Dollar"),
            new BingCurrency("Venezualan Bolivar"),
            new BingCurrency("Vietnamese Dong"),
            new BingCurrency("Zimbabwe Dollar")
        };

        #endregion

        public override ICurrency[] Currencies
        {
            get
            {
                return _currencies;
            }
        }

        protected override string CreateRequestUrl(double amount, ICurrency fromCurrency, ICurrency toCurrency)
        {
            return string.Format(@"http://www.bing.com/search?q={0}+{1}+in+{2}&scope=web&mkt=en-US&FORM=W0LH",
                amount,
                fromCurrency.Name.Replace(" ", "+"),
                toCurrency.Name.Replace(" ", "+"));
        }

        protected override double GetResultFromResponseContent(string responseContent)
        {
            var match = _resultRegex.Match(responseContent);

            if (match.Success)
                return double.Parse(match.Groups["value"].Value, CultureInfo.InvariantCulture);
            else
                throw new Exception("Conversion not returned!");
        }
    }
}
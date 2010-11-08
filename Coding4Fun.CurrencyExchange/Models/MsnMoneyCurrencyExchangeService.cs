using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Coding4Fun.CurrencyExchange.Model
{
    public class MsnMoneyCurrencyExchangeService : CurrencyExchangeServiceBase
    {
        #region Static Globals

        private static Regex _resultRegex = new Regex("<td class=\"ars s4\">(?<value>[0-9.,]+).*?</td>");

        private static ICurrency[] _currencies = new ICurrency[] { 
            new MsnMoneyCurrency("Algerian Dinar", 48),
            new MsnMoneyCurrency("Argentine Peso", 10),
            new MsnMoneyCurrency("Australian Dollar", 8),
            new MsnMoneyCurrency("Baharaini Dinar", 57),
            new MsnMoneyCurrency("Bolivian Boliviano", 13),
            new MsnMoneyCurrency("Brazilian Real", 14),
            new MsnMoneyCurrency("British Pound", 3),
            new MsnMoneyCurrency("Botswana Pula", 52),
            new MsnMoneyCurrency("Canadian Dollar", 2),
            new MsnMoneyCurrency("Chilean Peso", 16),
            new MsnMoneyCurrency("Chinese Yuan", 17),
            new MsnMoneyCurrency("Colombian Peso", 18),
            new MsnMoneyCurrency("Czech Koruna", 19),
            new MsnMoneyCurrency("Danish Krone", 20),
            new MsnMoneyCurrency("Egyptian Pound", 92),
            new MsnMoneyCurrency("Euro", 9),
            new MsnMoneyCurrency("Ghanaian Cedi", 53),
            new MsnMoneyCurrency("Guatemalan Quetzal", 45),
            new MsnMoneyCurrency("Hong Kong Dollar", 24),
            new MsnMoneyCurrency("Hungarian Forint", 25),
            new MsnMoneyCurrency("new Israeli shekel", 50),
            new MsnMoneyCurrency("Indian Rupee", 59),
            new MsnMoneyCurrency("Indonesian Rupiah", 26),
            new MsnMoneyCurrency("Japanese Yen", 7),
            new MsnMoneyCurrency("Jordanian Dinar", 60),
            new MsnMoneyCurrency("Kenyan Shilling", 54),
            new MsnMoneyCurrency("Korean Won", 28),
            new MsnMoneyCurrency("Kuwaiti Dinar", 61),
            new MsnMoneyCurrency("Malaysian Ringgit", 51),
            new MsnMoneyCurrency("Mexican Peso", 30),
            new MsnMoneyCurrency("Moroccan Dirham", 46),
            new MsnMoneyCurrency("Namibian Dollar", 55),
            new MsnMoneyCurrency("New Zealand Dollar", 32),
            new MsnMoneyCurrency("Norwegian Krone", 33),
            new MsnMoneyCurrency("Omani Rial", 63),
            new MsnMoneyCurrency("Pakistan Rupee", 64),
            new MsnMoneyCurrency("Peruvian Nuevo Sol", 34),
            new MsnMoneyCurrency("Philippine Peso", 65),
            new MsnMoneyCurrency("Qatari Rial", 66),
            new MsnMoneyCurrency("Russian Rouble", 37),
            new MsnMoneyCurrency("Saudi Riyal", 67),
            new MsnMoneyCurrency("Singapore Dollar", 40),
            new MsnMoneyCurrency("South African Rand", 44),
            new MsnMoneyCurrency("Swedish Krona", 39),
            new MsnMoneyCurrency("Swiss Franc", 15),
            new MsnMoneyCurrency("Taiwan Dollar", 42),
            new MsnMoneyCurrency("Tanzanian Shilling", 68),
            new MsnMoneyCurrency("Thai Baht", 41),
            new MsnMoneyCurrency("Tunisian Dinar", 47),
            new MsnMoneyCurrency("Turkish Lira", 49),
            new MsnMoneyCurrency("Emirati Dirham", 69),
            new MsnMoneyCurrency("US Dollar", 1),
            new MsnMoneyCurrency("Venezualan Bolivar", 43),
            new MsnMoneyCurrency("Vietnamese Dong", 70),
            new MsnMoneyCurrency("Zimbabwe Dollar", 56),
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
            return string.Format(@"http://moneycentral.msn.com/investor/market/currencyconverter.aspx?strAmt={0}&iSelectCurFrom={1}&iSelectCurTo={2}",
                amount,
                ((MsnMoneyCurrency)fromCurrency).Id,
                ((MsnMoneyCurrency)toCurrency).Id);
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
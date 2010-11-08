namespace Coding4Fun.CurrencyExchange.Model
{
    public class BingCurrency : ICurrency
    {
        #region Properties

        public string Name { get; protected set; }

        #endregion

        public BingCurrency(string name)
        {
            Name = name;
        }
    }
}
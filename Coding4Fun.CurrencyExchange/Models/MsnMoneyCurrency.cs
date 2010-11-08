namespace Coding4Fun.CurrencyExchange.Model
{
    public class MsnMoneyCurrency : ICurrency
    {
        #region Properties

        public string Name { get; protected set; }

        public int Id { get; protected set; }

        #endregion

        public MsnMoneyCurrency(string name, int id)
        {
            Name = name;
            Id = id;
        }
    }
}
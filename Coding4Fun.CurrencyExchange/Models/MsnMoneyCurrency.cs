namespace Coding4Fun.CurrencyExchange.Models
{
    public class MsnMoneyCurrency : CurrencyBase
    {
        #region Properties

        public int Id { get; protected set; }

        #endregion

        public MsnMoneyCurrency(string name, int id)
        {
            Name = name;
            Id = id;
        }
    }
}
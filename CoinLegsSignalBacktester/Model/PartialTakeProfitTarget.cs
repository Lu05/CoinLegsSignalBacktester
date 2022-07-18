namespace CoinLegsSignalBacktester.Model
{
    internal class PartialTakeProfitTarget
    {
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public bool IsHit { get; set; } = false;
        public PartialTakeProfitTarget(decimal price, decimal amount)
        {
            Price = price;
            Amount = amount;
        }
    }
}

namespace CoinLegsSignalBacktester.Backtest
{
    internal class BacktestPosition
    {
        public string Symbol { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PnL { get; set; }

        public BacktestPosition(string symbol, DateTime startDate, DateTime endDate, decimal pnL)
        {
            Symbol = symbol;
            StartDate = startDate;
            EndDate = endDate;
            PnL = pnL;
        }
    }
}

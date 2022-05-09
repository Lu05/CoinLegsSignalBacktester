namespace CoinLegsSignalBacktester.Backtest
{
    public class BacktestResult
    {
        public BackTestResultState State { get; set; }
        public decimal EntryPrice { get; set; }
        public decimal ExitPrice { get; set; }
        public bool StopLossHit { get; set; }
        public decimal PnL { get; set; }
        public decimal MaxLoss { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public enum BackTestResultState
    {
        Invalid,
        Valid
    }
}
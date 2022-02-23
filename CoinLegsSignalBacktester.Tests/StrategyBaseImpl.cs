using CoinLegsSignalBacktester.Backtest;
using CoinLegsSignalBacktester.Strategy;

namespace CoinLegsSignalBacktester.Tests
{
    internal class StrategyBaseImpl : StrategyBase
    {
        public override void Update(decimal price)
        {
            // ignore
        }

        public override void SetParameters(BacktestData data, BacktestConfig config)
        {
            TakeProfit = config.GetValue<decimal>("TakeProfit");
            StopLoss = config.GetValue<decimal>("StopLoss");
        }
    }
}
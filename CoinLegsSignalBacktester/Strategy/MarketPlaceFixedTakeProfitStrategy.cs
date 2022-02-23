using CoinLegsSignalBacktester.Backtest;

namespace CoinLegsSignalBacktester.Strategy
{
    internal class MarketPlaceFixedTakeProfitStrategy : StrategyBase
    {
        public override void Update(decimal price)
        {
        }

        public override void SetParameters(BacktestData data, BacktestConfig config)
        {
            if (config.HasParameter("TakeProfitIndex")) TakeProfit = (decimal)data.Notification.GetType().GetProperty($"Target{config.GetValue<int>("TakeProfitIndex")}")?.GetValue(data.Notification)!;

            if (config.HasParameter("UseStopLossFromSignal"))
            {
                var value = config.GetValue<bool>("UseStopLossFromSignal");
                if (value)
                {
                    StopLoss = data.Notification.StopLoss;
                }
            }
        }
    }
}
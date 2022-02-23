using CoinLegsSignalBacktester.Model;
using CoinLegsSignalBacktester.Plot;

namespace CoinLegsSignalBacktester.Backtest
{
    public class Backtester
    {
        private readonly Plotter _plotter = new();

        public void Run(IEnumerable<BacktestData> data, Config config, bool? plot = false)
        {
            var strategyConfig = config.BacktestConfigs.FirstOrDefault(c => c.StrategyName == config.StrategyToUse);
            if (strategyConfig == null)
            {
                Console.WriteLine($"Config for {config.StrategyToUse} not found!");
                return;
            }

            var strategy = StrategyHelper.GetStrategyByName(config.StrategyToUse);

            decimal profit = 0;
            decimal maxLoss = 0;
            int lossCount = 0;
            int profitCount = 0;
            foreach (var backtestData in data)
            {
                var result = strategy.Backtest(backtestData, strategyConfig);

                if (result.State == BackTestResultState.Valid)
                {
                    var line = $"{backtestData.Notification.MarketName} ===> profit {Math.Round(result.PnL * 100, 3)}% ===> max loss {Math.Round(result.MaxLoss * 100, 3)}%";
                    if (result.PnL > 0)
                    {
                        ColorConsole.WriteProfit(line);
                        profitCount++;
                    }
                    else
                    {
                        ColorConsole.WriteLoss(line);
                        lossCount++;
                    }
                    profit += result.PnL;
                    if (result.MaxLoss < maxLoss) maxLoss = result.MaxLoss;
                }
                else
                {
                    ColorConsole.WriteWarning($"{backtestData.Notification.MarketName} ===> invalid!!!");
                }

                if (plot is true)
                {
                    _plotter.Plot(strategy, backtestData, result.State);
                }
            }

            ColorConsole.WriteInfo($"===> profit {Math.Round(profit * 100, 3)}% ===> max loss {Math.Round(maxLoss * 100, 3)}%");
            ColorConsole.WriteInfo($"wins: {profitCount} === losses: {lossCount}");
        }
    }
}
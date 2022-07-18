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
            var btData = data.ToList();
            var positionManager = new PositionManager(config.MaxParallelPositions, config.CoolDownPeriod)
            {
                WriteOutputToConsole = true
            };
            foreach (var backtestData in btData)
            {
                if (!positionManager.CanExecute(backtestData))
                {
                    continue;
                }

                var result = strategy.Backtest(backtestData, strategyConfig);

                if (result.State == BackTestResultState.Valid)
                {
                    var line = $"{backtestData.Notification.MarketName} ===> profit {Math.Round(result.PnL * 100, 3)}% ===> max loss {Math.Round(result.MaxLoss * 100, 3)}%";
                    if (backtestData.Version > 1)
                    {
                        line += $" ===> duration {Math.Round(result.Duration.TotalMinutes, 0)} min";
                        positionManager.AddPosition(backtestData.Notification.MarketName, backtestData.Date, backtestData.Date.Add(result.Duration), result.PnL);
                    }
                    else
                    {
                        positionManager.AddPosition(backtestData.Notification.MarketName, backtestData.Date, backtestData.Date.AddDays(1), result.PnL);
                    }
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
                    if (result.MaxLoss < maxLoss)
                    {
                        maxLoss = result.MaxLoss;
                    }
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

            ColorConsole.WriteInfo($"profit {Math.Round(profit * 100, 3)}%");
            ColorConsole.WriteInfo($"max loss {Math.Round(maxLoss * 100, 3)}%");
            ColorConsole.WriteInfo($"wins: {profitCount} === losses: {lossCount}");
            var dates = btData.OrderBy(d => d.Date).ToList();
            var days = Math.Max((dates.Last().Date - dates.First().Date).TotalDays, 1);
            ColorConsole.WriteInfo($"days {Math.Round(days, 2)}");
            ColorConsole.WriteInfo($"avg daily profit {Math.Round((profit/(decimal)days) * 100, 3)}%");
        }
        
    }
}
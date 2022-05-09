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
            var closedPositions = new Dictionary<string, List<Tuple<DateTime, DateTime>>>();
            foreach (var backtestData in btData)
            {
                if (IsPositionAlreadyOpen(closedPositions, backtestData))
                {
                    ColorConsole.WriteWarning($"Position for {backtestData.Notification.MarketName} already open!");
                    continue;
                }

                var result = strategy.Backtest(backtestData, strategyConfig);

                if (result.State == BackTestResultState.Valid)
                {
                    var line = $"{backtestData.Notification.MarketName} ===> profit {Math.Round(result.PnL * 100, 3)}% ===> max loss {Math.Round(result.MaxLoss * 100, 3)}%";
                    if (backtestData.Version > 1)
                    {
                        line += $" ===> duration {Math.Round(result.Duration.TotalMinutes, 0)} min";
                        AddToClosedPositions(backtestData, closedPositions, result.Duration);
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
            var days = (dates.Last().Date - dates.First().Date).TotalDays;
            ColorConsole.WriteInfo($"days {Math.Round(days, 2)}");
            ColorConsole.WriteInfo($"avg daily profit {Math.Round((profit/(decimal)days) * 100, 3)}%");
        }

        /// <summary>
        /// Adds the backtest result to the closed positions
        /// </summary>
        /// <param name="backtestData">data to add</param>
        /// <param name="closedPositions">list of all closed positions</param>
        /// <param name="duration">duration of the backtest</param>
        private static void AddToClosedPositions(BacktestData backtestData, Dictionary<string, List<Tuple<DateTime, DateTime>>> closedPositions, TimeSpan duration)
        {
            var openClosed = new Tuple<DateTime, DateTime>(backtestData.Date, backtestData.Date.Add(duration));
            if (closedPositions.TryGetValue(backtestData.Notification.MarketName, out List<Tuple<DateTime, DateTime>> positions))
            {
                positions.Add(openClosed);
            }
            else
            {
                closedPositions.Add(backtestData.Notification.MarketName, new List<Tuple<DateTime, DateTime>>
                {
                    openClosed
                });
            }
        }

        /// <summary>
        /// Checks if a position is already open for the same symbol
        /// </summary>
        /// <param name="closedPositions">all closed positions</param>
        /// <param name="data">current backtest data</param>
        /// <returns>true, if the position is already open, otherwise false</returns>
        private bool IsPositionAlreadyOpen(Dictionary<string, List<Tuple<DateTime, DateTime>>> closedPositions, BacktestData data)
        {
            if (!closedPositions.TryGetValue(data.Notification.MarketName, out List<Tuple<DateTime, DateTime>> positions))
            {
                return false;
            }

            for (int i = positions.Count - 1; i >= 0; i--)
            {
                var current = positions[i];
                if (data.Date > current.Item1 && data.Date < current.Item2)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
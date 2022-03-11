using CoinLegsSignalBacktester.Backtest;
using CoinLegsSignalBacktester.Model;
using CoinLegsSignalBacktester.Strategy;
using Newtonsoft.Json;

namespace CoinLegsSignalBacktester.Optimize;

internal class Optimizer
{
    private bool _run;

    public void Run(IEnumerable<BacktestData> data, Config config, OptimizationTarget target)
    {
        _run = true;

        var optimizationConfig = config.OptimizationtConfigs.FirstOrDefault(c => c.StrategyName == config.StrategyToUse);
        if (optimizationConfig == null)
        {
            Console.WriteLine($"Optimization config not found {config.StrategyToUse}");
            return;
        }

        var lockObj = new object();
        decimal maxProfit = 0;
        int maxWins = 0;
        BacktestConfig bestConfig;
        Parallel.ForEach(Infinite(), new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount - 1
        }, _ =>
        {
            var strategy = StrategyHelper.GetStrategyByName(config.StrategyToUse);
            if (strategy == null)
            {
                Console.WriteLine($"Strategy {config.StrategyToUse} not found");
                Stop();
                return;
            }

            var rnd = new ParameterRandomizer();
            var btConfig = new BacktestConfig
            {
                Parameters = optimizationConfig.BacktestParameters.ToList()
            };
            foreach (var optimizationParam in optimizationConfig.OptimizationParameters)
            {
                var value = rnd.GetNextValue(optimizationParam);
                btConfig.Parameters.Add(new BacktestParameter
                {
                    Key = optimizationParam.Key,
                    Value = value
                });
            }

            var backtestDataArray = data.ToArray();
            var result = Backtest(strategy, backtestDataArray, btConfig);
            lock (lockObj)
            {
                if (target == OptimizationTarget.Profit)
                {
                    if (result.Item1 > maxProfit)
                    {
                        maxProfit = result.Item1;
                        bestConfig = btConfig;
                        Console.WriteLine($"==> {Math.Round(maxProfit * 100, 3)}%");
                        Console.WriteLine(JsonConvert.SerializeObject(bestConfig, Formatting.Indented));
                    }
                }
                else if (target == OptimizationTarget.Wins)
                {
                    if (result.Item2 >= maxWins && result.Item1 > maxProfit)
                    {
                        maxWins = result.Item2;
                        maxProfit = result.Item1;
                        bestConfig = btConfig;
                        Console.WriteLine($"==> wins {maxWins}/{backtestDataArray.Length} ==> profit {Math.Round(result.Item1 * 100, 3)}%");
                        Console.WriteLine(JsonConvert.SerializeObject(bestConfig, Formatting.Indented));
                    }
                }
            }
        });
    }

    private Tuple<decimal, int> Backtest(StrategyBase strategy, IEnumerable<BacktestData> data, BacktestConfig config)
    {
        decimal profit = 0;
        int wins = 0;
        foreach (var backtestData in data)
        {
            var result = strategy.Backtest(backtestData, config);
            profit += result.PnL;
            if (result.PnL > 0)
            {
                wins++;
            }
        }

        return new Tuple<decimal, int>(profit, wins);
    }

    private IEnumerable<bool> Infinite()
    {
        while (_run) yield return true;
    }

    public void Stop()
    {
        _run = false;
    }
}
using CoinLegsSignalBacktester.Backtest;
using CoinLegsSignalBacktester.Model;
using CoinLegsSignalBacktester.Strategy;
using Newtonsoft.Json;

namespace CoinLegsSignalBacktester.Optimize;

internal class Optimizer
{
    private bool _run;

    public void Run(IEnumerable<BacktestData> data, Config config)
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
        BacktestConfig bestConfig;
        Parallel.ForEach(Infinite(), _ =>
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

            var result = Backtest(strategy, data, btConfig);
            lock (lockObj)
            {
                if (result > maxProfit)
                {
                    maxProfit = result;
                    bestConfig = btConfig;
                    Console.WriteLine($"==> {Math.Round(maxProfit * 100, 3)}%");
                    Console.WriteLine(JsonConvert.SerializeObject(bestConfig, Formatting.Indented));
                }
            }
        });
    }

    private decimal Backtest(StrategyBase strategy, IEnumerable<BacktestData> data, BacktestConfig config)
    {
        decimal profit = 0;
        foreach (var backtestData in data)
        {
            var result = strategy.Backtest(backtestData, config);
            profit += result.PnL;
        }

        return profit;
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
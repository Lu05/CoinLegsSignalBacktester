using System.CommandLine;
using System.Globalization;
using CoinLegsSignalBacktester.Backtest;
using CoinLegsSignalBacktester.Model;
using CoinLegsSignalBacktester.Optimize;
using Newtonsoft.Json;

namespace CoinLegsSignalBacktester;

public class Program
{
    private static readonly Optimizer Optimizer = new();

    public static void Main(string[] args)
    {
        var configArg = new Option<string>(new[] { "-c", "--config" }, () => string.Empty, "the path to the config file");
        var plotOption = new Option<bool>(new[] { "-p", "--plot" }, () => false, "plot html files for each backtest");
        var daysOption = new Option<int>(new[] { "-d", "--days" }, () => int.MaxValue, "max days (now - days) uses for backtest/optimize");
        var optimizeTargetOption = new Option<string>(new[] { "-t", "--target" }, () => "profit", "target for optimization (profit, wins)");

        var root = new RootCommand();

        var backtestCommand = new Command("backtest")
        {
            configArg,
            plotOption,
            daysOption
        };
        root.Add(backtestCommand);

        backtestCommand.SetHandler((string configPath, bool plot, int days) =>
            {
                ExecuteBacktest(configPath, plot, days);
            }
            , configArg, plotOption, daysOption);

        var optimizeCommand = new Command("optimize")
        {
            configArg,
            daysOption,
            optimizeTargetOption
        };
        root.Add(optimizeCommand);
        optimizeCommand.SetHandler((string configPath, int days, string target) =>
            {
                ExecuteOptimize(configPath, days, target);
            }
            , configArg, daysOption, optimizeTargetOption);

        if (args.Length == 0)
            root.Invoke("-h");
        else
            root.Invoke(args);

        while (true)
        {
            var input = Console.ReadLine();
            if (input != null && input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

            if (input != null)
                root.Invoke(input);
        }
    }

    private static void ExecuteOptimize(string configPath, int days, string target)
    {
        var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
        if (config != null)
        {
            var optimizeTarget = OptimizationTarget.Profit;
            if (target == "wins")
            {
                optimizeTarget = OptimizationTarget.Wins;
            }
            var data = LoadData(config.DataPath, days);
            Optimizer.Run(data, config, optimizeTarget);
        }
    }

    private static IEnumerable<BacktestData> LoadData(string directory, int days)
    {
        var files = Directory.GetFiles(directory);
        DateTime? maxDate = null;
        if (days < int.MaxValue) maxDate = DateTime.Now.Subtract(TimeSpan.FromDays(days));

        foreach (var file in files)
        {
            var data = JsonConvert.DeserializeObject<BacktestData>(File.ReadAllText(file));
            if (data == null) continue;

            var fileName = Path.GetFileNameWithoutExtension(file);
            data.FileName = fileName;
            if (maxDate != null)
            {
                var date = DateTime.ParseExact(fileName.Split('_')[2], "yyyyMMddhhmmss", CultureInfo.InvariantCulture);
                if (date > maxDate) yield return data;

                continue;
            }

            yield return data;
        }
    }

    private static void ExecuteBacktest(string configPath, bool plot, int days)
    {
        var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
        if (config != null)
        {
            var data = LoadData(config.DataPath, days);
            var bt = new Backtester();
            bt.Run(data, config, plot);
        }
    }
}
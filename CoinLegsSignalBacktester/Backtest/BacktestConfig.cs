namespace CoinLegsSignalBacktester.Backtest;

public class BacktestConfig
{
    public List<BacktestParameter> Parameters { get; set; }
    public string StrategyName { get; set; }

    public T GetValue<T>(string parameterName)
    {
        var param = Parameters.FirstOrDefault(p => p.Key == parameterName);
        if (param != null)
        {
            return (T)Convert.ChangeType(param.Value, typeof(T));
        }
        Console.WriteLine($"Parameter {parameterName} not found!");
        return default;
    }

    public bool HasParameter(string parameterName)
    {
        return Parameters.Any(p => p.Key == parameterName);
    }
}
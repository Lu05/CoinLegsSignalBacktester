using CoinLegsSignalBacktester.Backtest;

namespace CoinLegsSignalBacktester.Optimize
{
    public class OptimizationConfig
    {
        public List<OptimizationParameter> OptimizationParameters { get; set; }
        public List<BacktestParameter> BacktestParameters { get; set; }
        public string StrategyName { get; set; }
    }
}
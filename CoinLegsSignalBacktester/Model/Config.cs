using CoinLegsSignalBacktester.Backtest;
using CoinLegsSignalBacktester.Optimize;

namespace CoinLegsSignalBacktester.Model
{
    public class Config
    {
        public List<BacktestConfig> BacktestConfigs { get; set; }
        public List<OptimizationConfig> OptimizationtConfigs { get; set; }
        public string StrategyToUse { get; set; }
        public string DataPath { get; set; }
        public int MaxParallelPositions { get; set; }
        public CoolDownPeriodConfig CoolDownPeriod { get; set; }
    }
}
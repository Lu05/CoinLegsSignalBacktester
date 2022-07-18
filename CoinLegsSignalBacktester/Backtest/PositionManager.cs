using CoinLegsSignalBacktester.Model;

namespace CoinLegsSignalBacktester.Backtest
{
    internal class PositionManager
    {
        private readonly int _maxParallelPositions;
        private readonly CoolDownPeriodConfig _coolDownPeriod;
        private readonly List<BacktestPosition> _positions = new();
        public bool WriteOutputToConsole { get; set; }

        /// <summary>
        /// Manager class for closed Positions to manage execution
        /// </summary>
        /// <param name="maxParallelPositions">Max allowed open positions</param>
        /// <param name="coolDownPeriodConfig">Cool down period  to pause the bot</param>
        public PositionManager(int maxParallelPositions, CoolDownPeriodConfig coolDownPeriodConfig)
        {
            _maxParallelPositions = maxParallelPositions;
            _coolDownPeriod = coolDownPeriodConfig;
        }

        private bool IsMaxParallelPositionsReached(BacktestData data)
        {
            if (_maxParallelPositions == 0)
                return false;
            int count = 0;
            foreach (var closedPosition in _positions)
            {
                if (closedPosition.StartDate < data.Date && closedPosition.EndDate > data.Date)
                {
                    count++;
                    if (count == _maxParallelPositions)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Adds a closed position
        /// </summary>
        /// <param name="symbolName">Symbol name</param>
        /// <param name="startDate">Start date of the position</param>
        /// <param name="endDate">End date of the position</param>
        /// <param name="pnl">Profit/Loss</param>
        public void AddPosition(string symbolName, DateTime startDate, DateTime endDate, decimal pnl)
        {
            _positions.Add(new BacktestPosition(symbolName, startDate, endDate, pnl));
        }

        private bool IsPositionAlreadyOpened(BacktestData data)
        {
            var positions = _positions.Where(p => p.Symbol == data.Notification.MarketName).ToList();
            if (!positions.Any())
            {
                return false;
            }

            return positions.Any(p => data.Date > p.StartDate && data.Date < p.EndDate);
        }

        private bool IsCoolDownPeriodActive(BacktestData data)
        {
            if (_coolDownPeriod.CoolDownHours == 0)
            {
                return false;
            }
            
            if (_positions.Count < _coolDownPeriod.PositionCount)
            {
                return false;
            }
            var closedPositions = _positions.OrderBy(p => p.EndDate).TakeLast(_coolDownPeriod.PositionCount).ToList();
            if (closedPositions.Sum(c => c.PnL) < -_coolDownPeriod.MaxDrawdown)
            {
                if (closedPositions.Last().EndDate.AddHours(_coolDownPeriod.CoolDownHours) > data.Date)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the position is allowed to execute
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>True if execution is allowed, otherwise false</returns>
        public bool CanExecute(BacktestData data)
        {
            if (IsCoolDownPeriodActive(data))
            {
                if(WriteOutputToConsole)
                {
                    ColorConsole.WriteWarning("Cool down perdiod active!");
                }
                return false;
            }
            if (IsMaxParallelPositionsReached(data))
            {
                if(WriteOutputToConsole)
                {
                    ColorConsole.WriteWarning($"Max positions reached {_maxParallelPositions}!");
                }
                return false;
            }
            if (IsPositionAlreadyOpened(data))
            {
                if(WriteOutputToConsole)
                {
                    ColorConsole.WriteWarning($"Position for {data.Notification.MarketName} is already open!");
                }
                return false;
            }

            return true;
        }
    }
}
using CoinLegsSignalBacktester.Backtest;

namespace CoinLegsSignalBacktester.Strategy
{
    internal class MarketPlaceTrailingStopLossStrategy : StrategyBase
    {
        private decimal _trailingOffset;
        private decimal _trailingStartOffset;
        private bool _isTrailingActive;

        public override void Update(decimal price)
        {
            if(!IsPositionOpen)
                return;
            if (IsShort)
            {
                if (!_isTrailingActive)
                {
                    var offset = 1 - price / EntryPrice;
                    if (offset > _trailingStartOffset)
                    {
                        _isTrailingActive = true;
                    }
                }
                if(!_isTrailingActive)
                    return;
                var sl = price + price * _trailingOffset;
                if (StopLoss > sl)
                {
                    StopLoss = sl;
                }
            }
            else
            {
                if (!_isTrailingActive)
                {
                    var offset = price / EntryPrice - 1;
                    if (offset > _trailingStartOffset)
                    {
                        _isTrailingActive = true;
                    }
                }
                if(!_isTrailingActive)
                    return;
                var sl = price + price * _trailingOffset;
                if (StopLoss < sl)
                {
                    StopLoss = sl;
                }
            }
        }

        public override void SetParameters(BacktestData data, BacktestConfig config)
        {
            if (config.HasParameter("UseStopLossFromSignal"))
            {
                var value = config.GetValue<bool>("UseStopLossFromSignal");
                if (value)
                {
                    StopLoss = data.Notification.StopLoss;
                }
            }

            _trailingOffset = config.GetValue<decimal>("TrailingOffset");
            _trailingStartOffset = config.GetValue<decimal>("TrailingStartOffset");
        }
    }
}

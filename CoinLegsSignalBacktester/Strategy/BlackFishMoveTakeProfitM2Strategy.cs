using CoinLegsSignalBacktester.Backtest;

namespace CoinLegsSignalBacktester.Strategy
{
    internal class BlackFishMoveTakeProfitM2Strategy : StrategyBase
    {
        private decimal _tp1;
        private decimal _tp2;
        private decimal _tp3;
        private decimal _tp4;
        private decimal _tp5;
        private decimal _sl;
        private decimal _signalPrice;

        public override void Update(decimal price)
        {
            if (IsShort)
            {
                if (price < _tp4)
                {
                    _sl = _tp3;
                }
                else if (price < _tp3)
                {
                    _sl = _tp1;
                }
                else if (price < _tp2)
                {
                    _sl = _signalPrice;
                }

                if (_sl <= StopLoss)
                {
                    StopLoss = _sl;
                }
            }
            else
            {
                if (price > _tp4)
                {
                    _sl = _tp3;
                }
                else if (price > _tp3)
                {
                    _sl = _tp1;
                }
                else if (price > _tp2)
                {
                    _sl = _signalPrice;
                }

                if (_sl >= StopLoss)
                {
                    StopLoss = _sl;
                }
            }
        }

        public override void SetParameters(BacktestData data, BacktestConfig config)
        {
            TakeProfit = data.Notification.Target5;
            StopLoss = data.Notification.StopLoss;

            _tp1 = data.Notification.Target1;
            _tp2 = data.Notification.Target2;
            _tp3 = data.Notification.Target3;
            _tp4 = data.Notification.Target4;
            _tp5 = data.Notification.Target5;
            _sl = data.Notification.StopLoss;
            _signalPrice = data.Notification.SignalPrice;
        }
    }
}
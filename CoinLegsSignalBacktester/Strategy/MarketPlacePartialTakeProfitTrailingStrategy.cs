using CoinLegsSignalBacktester.Backtest;
using CoinLegsSignalBacktester.Model;
using CoinLegsSignalBacktester.Model.CoinLegsSignalDataCollector.Model;
using Newtonsoft.Json.Linq;

namespace CoinLegsSignalBacktester.Strategy
{
    internal class MarketPlacePartialTakeProfitTrailingStrategy : StrategyBase
    {
        private decimal _trailingOffset;
        private decimal _trailingStopPrice;
        private decimal _trailingExitPrice;
        private bool _isTrailingActive;
        private readonly List<PartialTakeProfitTarget> _profits = new();
        private bool _trailingTargetHit;

        public override void Update(decimal price)
        {
            if(!IsPositionOpen)
                return;
            if (IsShort)
            {
                foreach (var target in _profits)
                {
                    if(target.IsHit)
                        continue;
                    if (price <= target.Price)
                    {
                        target.IsHit = true;
                    }
                }

                if (_isTrailingActive)
                {
                    var trailingStop = price + price * _trailingOffset;
                    if(trailingStop < _trailingStopPrice)
                    {
                        _trailingStopPrice = trailingStop;
                    }

                    if (price > _trailingStopPrice)
                    {
                        _trailingExitPrice = price;
                        _trailingTargetHit = true;
                        IsPositionOpen = false;
                        BackTestResult.State = BackTestResultState.Valid;
                        BackTestResult.ExitPrice = _trailingExitPrice;
                    }
                }

                if (_profits.All(p => p.IsHit))
                {
                    _isTrailingActive = true;
                    _trailingStopPrice = price + price * _trailingOffset;
                }
            }
            else
            {
                foreach (var target in _profits)
                {
                    if(target.IsHit)
                        continue;
                    if (price >= target.Price)
                    {
                        target.IsHit = true;
                    }
                }

                if (_isTrailingActive)
                {
                    var trailingStop = price - price * _trailingOffset;
                    if(trailingStop > _trailingStopPrice)
                    {
                        _trailingStopPrice = trailingStop;
                    }

                    if (price < _trailingStopPrice)
                    {
                        _trailingExitPrice = price;
                        _trailingTargetHit = true;
                        IsPositionOpen = false;
                        BackTestResult.State = BackTestResultState.Valid;
                        BackTestResult.ExitPrice = _trailingExitPrice;
                    }
                }

                if (_profits.All(p => p.IsHit))
                {
                    _isTrailingActive = true;
                    _trailingStopPrice = price - price * _trailingOffset;
                }
            }
        }

        protected override decimal CalculatePnL()
        {
            decimal profit = 0;
            foreach (var item in _profits)
            {
                if (item.IsHit)
                {
                    if (IsShort)
                    {
                        profit += (1 - item.Price / EntryPrice) * item.Amount;
                    }
                    else
                    {
                        profit += (1 - EntryPrice / item.Price) * item.Amount;
                    }
                }
                else
                {
                    break;
                }
            }

            if (_trailingTargetHit)
            {
                var trailingAmount = 1 - _profits.Sum(p => p.Amount);
                if (IsShort)
                {
                    profit += (1 - _trailingExitPrice / _profits.Last().Price) * trailingAmount;
                }
                else
                {
                    profit += (1 - _profits.Last().Price / _trailingExitPrice) * trailingAmount;
                }
            }
            else if (BackTestResult.StopLossHit)
            {
                var restAmount = 1 - _profits.Where(p => p.IsHit).Sum(p => p.Amount);
                if (IsShort)
                {
                    profit += (1 - StopLoss / EntryPrice) * restAmount;
                }
                else
                {
                    profit += (1 - EntryPrice / StopLoss) * restAmount;
                }
            }

            if (!BackTestResult.StopLossHit && !_trailingTargetHit)
            {
                var restAmount = 1 - _profits.Where(p => p.IsHit).Sum(p => p.Amount);
                if (IsShort)
                {
                    profit += (1 - BackTestResult.ExitPrice / EntryPrice) * restAmount;
                }
                else
                {
                    profit += (1 - EntryPrice / BackTestResult.ExitPrice) * restAmount;
                }
            }

            return profit;
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

            var profitValues = config.GetValue<JArray>("Profits");
            _profits.Clear();
            foreach (var profitValue in profitValues)
            {
                var price = GetTargetFromNotification(profitValue["Index"]!.ToObject<int>(), data.Notification);
                _profits.Add(new PartialTakeProfitTarget(price, profitValue["Amount"]!.ToObject<decimal>()));
            }

            if (_profits.Sum(p => p.Amount) > 1.001m)
            {
                throw new Exception("Profit amount can not be greater than 1!");
            }

            _trailingExitPrice = 0;
            _trailingOffset = config.GetValue<decimal>("TrailingOffset");
            // _trailingStartOffset = config.GetValue<decimal>("TrailingStartOffset");
            _isTrailingActive = false;
            _trailingTargetHit = false;
            _trailingStopPrice = 0;
        }

        private decimal GetTargetFromNotification(int targetIndex, Notification notification)
        {
            return targetIndex switch
            {
                1 => notification.Target1,
                2 => notification.Target2,
                3 => notification.Target3,
                4 => notification.Target4,
                5 => notification.Target5,
                _ => throw new Exception($"Index {targetIndex} not valid!")
            };
        }
    }
}

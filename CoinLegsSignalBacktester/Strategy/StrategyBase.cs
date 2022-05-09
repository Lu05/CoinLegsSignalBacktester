using CoinLegsSignalBacktester.Backtest;

namespace CoinLegsSignalBacktester.Strategy;

public abstract class StrategyBase
{
    private BacktestResult _backTestResult;
    protected bool IsPositionOpen { get; set; }
    protected decimal EntryPrice => _backTestResult.EntryPrice;
    public bool UseStopLoss { get; set; }
    public decimal TakeProfit { get; set; }
    public decimal StopLoss { get; set; }
    public bool IsShort { get; set; }
    public bool MarketBuy { get; set; }
    public int StartIndex { get; set; }
    public int LastIndex { get; set; }

    protected virtual void TryOpenOrExit(decimal price)
    {
        if (!IsPositionOpen)
        {
            TryOpenPosition(price);
        }
        else
        {
            if (IsShort)
            {
                if (price <= TakeProfit)
                {
                    _backTestResult.ExitPrice = price;
                    _backTestResult.State = BackTestResultState.Valid;
                    IsPositionOpen = false;
                }
                else if (UseStopLoss && price >= StopLoss)
                {
                    _backTestResult.ExitPrice = price;
                    _backTestResult.State = BackTestResultState.Valid;
                    IsPositionOpen = false;
                    _backTestResult.StopLossHit = true;
                }
            }
            else
            {
                if (price >= TakeProfit)
                {
                    _backTestResult.ExitPrice = price;
                    _backTestResult.State = BackTestResultState.Valid;
                    IsPositionOpen = false;
                }
                else if (UseStopLoss && price <= StopLoss)
                {
                    _backTestResult.ExitPrice = price;
                    _backTestResult.State = BackTestResultState.Valid;
                    IsPositionOpen = false;
                    _backTestResult.StopLossHit = true;
                }
            }
        }
    }

    private void TryOpenPosition(decimal price)
    {
        if (IsPositionOpen)
            return;

        if (IsShort)
        {
            if (price >= _backTestResult.EntryPrice)
            {
                IsPositionOpen = true;
                StartIndex = LastIndex;
            }
        }
        else
        {
            if (price <= _backTestResult.EntryPrice)
            {
                IsPositionOpen = true;
                StartIndex = LastIndex;
            }
        }
    }

    /// <summary>
    /// Will be called for each price at the data set 
    /// </summary>
    /// <param name="price"></param>
    public abstract void Update(decimal price);

    private decimal CalculateTpPercentage(decimal source, decimal percentage, bool isShort)
    {
        if (isShort)
            return source - source * percentage;
        return source + source * percentage;
    }

    private decimal CalculateSlPercentage(decimal source, decimal percentage, bool isShort)
    {
        if (isShort)
            return source + source * percentage;
        return source - source * percentage;
    }

    /// <summary>
    /// Will be called at start so you can read the params you need from the config
    /// </summary>
    /// <param name="data">Data for backtests</param>
    /// <param name="config">Config file</param>
    public abstract void SetParameters(BacktestData data, BacktestConfig config);

    public BacktestResult Backtest(BacktestData data, BacktestConfig config)
    {
        IsPositionOpen = false;
        _backTestResult = new BacktestResult
        {
            State = BackTestResultState.Invalid
        };
        IsShort = data.Notification.Signal < 0;
        StopLoss = CalculateSlPercentage(data.Notification.SignalPrice, config.GetValue<decimal>("StopLoss"), IsShort);
        TakeProfit = CalculateTpPercentage(data.Notification.SignalPrice, config.GetValue<decimal>("TakeProfit"), IsShort);
        UseStopLoss = config.GetValue<bool>("UseStopLoss");
        MarketBuy = config.GetValue<bool>("MarketBuy");
        SetParameters(data, config);
        if (MarketBuy)
        {
            _backTestResult.EntryPrice = data.LastPrice;
            IsPositionOpen = true;
            StartIndex = 0;
        }
        else
        {
            // ReSharper disable once PossibleInvalidOperationException
            _backTestResult.EntryPrice = data.Notification.SignalPrice;
        }

        LastIndex = -1;
        foreach (var price in data.Data)
        {
            LastIndex++;
            var lastPosOpen = IsPositionOpen;
            TryOpenOrExit(price);
            if (_backTestResult.State == BackTestResultState.Valid)
            {
                break;
            }

            if (lastPosOpen != IsPositionOpen)
            {
                continue;
            }

            Update(price);
        }

        if (_backTestResult.State == BackTestResultState.Invalid && IsPositionOpen)
        {
            _backTestResult.ExitPrice = data.Data.Last();
            _backTestResult.State = BackTestResultState.Valid;
            if (data.Version > 1)
            {
                _backTestResult.Duration = data.Times.Last() - data.Times.First();
            }
        }

        if (_backTestResult.State == BackTestResultState.Valid)
        {
            _backTestResult.PnL = GetPnL(_backTestResult.EntryPrice, _backTestResult.ExitPrice, IsShort);
            if (data.Version > 1)
            {
                _backTestResult.Duration = data.Times[LastIndex] - data.Times.First();
            }
        }

        _backTestResult.MaxLoss = GetMaxDrawLoss(data.Data, IsShort, _backTestResult.EntryPrice, LastIndex);

        return _backTestResult;
    }

    private decimal GetMaxDrawLoss(List<decimal> data, bool isShort, decimal entryPrice, int lastIndex)
    {
        if (isShort)
        {
            var maxPrice = data.Take(lastIndex).Max();
            return GetPnL(entryPrice, maxPrice, true);
        }

        var minPrice = data.Take(lastIndex).Min();
        return GetPnL(entryPrice, minPrice, false);
    }

    private decimal GetPnL(decimal entryPrice, decimal lastPrice, bool isShort)
    {
        if (isShort)
            return 1 - lastPrice / entryPrice;
        return -(1 - lastPrice / entryPrice);
    }
}
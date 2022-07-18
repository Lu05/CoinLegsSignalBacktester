using CoinLegsSignalBacktester.Backtest;

namespace CoinLegsSignalBacktester.Strategy;

public abstract class StrategyBase
{
    protected BacktestResult BackTestResult;
    protected bool IsPositionOpen { get; set; }
    protected decimal EntryPrice => BackTestResult.EntryPrice;
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
                    BackTestResult.ExitPrice = price;
                    BackTestResult.State = BackTestResultState.Valid;
                    IsPositionOpen = false;
                }
                else if (UseStopLoss && price >= StopLoss)
                {
                    BackTestResult.ExitPrice = price;
                    BackTestResult.State = BackTestResultState.Valid;
                    IsPositionOpen = false;
                    BackTestResult.StopLossHit = true;
                }
            }
            else
            {
                if (price >= TakeProfit)
                {
                    BackTestResult.ExitPrice = price;
                    BackTestResult.State = BackTestResultState.Valid;
                    IsPositionOpen = false;
                }
                else if (UseStopLoss && price <= StopLoss)
                {
                    BackTestResult.ExitPrice = price;
                    BackTestResult.State = BackTestResultState.Valid;
                    IsPositionOpen = false;
                    BackTestResult.StopLossHit = true;
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
            if (price >= BackTestResult.EntryPrice)
            {
                IsPositionOpen = true;
                StartIndex = LastIndex;
            }
        }
        else
        {
            if (price <= BackTestResult.EntryPrice)
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
        BackTestResult = new BacktestResult
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
            BackTestResult.EntryPrice = data.LastPrice;
            IsPositionOpen = true;
            StartIndex = 0;
        }
        else
        {
            // ReSharper disable once PossibleInvalidOperationException
            BackTestResult.EntryPrice = data.Notification.SignalPrice;
        }

        LastIndex = -1;
        foreach (var price in data.Data)
        {
            LastIndex++;
            var lastPosOpen = IsPositionOpen;
            TryOpenOrExit(price);
            if (BackTestResult.State == BackTestResultState.Valid)
            {
                break;
            }

            if (lastPosOpen != IsPositionOpen)
            {
                continue;
            }

            Update(price);
        }

        if (BackTestResult.State == BackTestResultState.Invalid && IsPositionOpen)
        {
            BackTestResult.ExitPrice = data.Data.Last();
            BackTestResult.State = BackTestResultState.Valid;
            if (data.Version > 1)
            {
                BackTestResult.Duration = data.Times.Last() - data.Times.First();
            }
        }

        if (BackTestResult.State == BackTestResultState.Valid)
        {
            BackTestResult.PnL = CalculatePnL();
            if (data.Version > 1)
            {
                BackTestResult.Duration = data.Times[LastIndex] - data.Times.First();
            }
        }

        BackTestResult.MaxLoss = GetMaxDrawLoss(data.Data, IsShort, BackTestResult.EntryPrice, LastIndex);

        return BackTestResult;
    }

    protected virtual decimal CalculatePnL()
    {
        return GetPnL(BackTestResult.EntryPrice, BackTestResult.ExitPrice, IsShort);
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
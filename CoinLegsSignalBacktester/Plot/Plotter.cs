using CoinLegsSignalBacktester.Backtest;
using CoinLegsSignalBacktester.CoinLegsSignalDataCollector;
using CoinLegsSignalBacktester.Strategy;
using Plotly.NET;
using Plotly.NET.LayoutObjects;

namespace CoinLegsSignalBacktester.Plot
{
    internal class Plotter
    {
        public void Plot(StrategyBase strategy, BacktestData data, BackTestResultState state)
        {
            var xvalues = Enumerable.Range(0, data.Data.Count).ToList();
            var yValues = data.Data;
            var title = state == BackTestResultState.Invalid ? $"{data.Notification.MarketName} --- invalid" : data.Notification.MarketName;

            var chart = Chart2D.Chart.Line<int, decimal, decimal>(xvalues, yValues)
                .WithXAxisStyle(null, ShowGrid: false, ShowLine: true)
                .WithYAxisStyle(Title.init("price"), ShowGrid: false, ShowLine: true)
                .WithLayout(Layout.init<string>(Title.init(title), false, null, null, true, 0, 700));

            var signalPriceLine = Shape.init<int, int, decimal, decimal>(StyleParam.ShapeType.Line, 0, xvalues.Count, data.Notification.SignalPrice, data.Notification.SignalPrice, null, 1,
                Line.init(null, null, null, null, null, Color.fromRGB(70, 212, 227)), null,
                StyleParam.Layer.Below);
            var stopLossLine = Shape.init<int, int, decimal, decimal>(StyleParam.ShapeType.Line, 0, xvalues.Count, strategy.StopLoss, strategy.StopLoss, null, 1,
                Line.init(null, null, null, null, null, Color.fromRGB(222, 0, 63)), null,
                StyleParam.Layer.Below);
            var takeProfitLine = Shape.init<int, int, decimal, decimal>(StyleParam.ShapeType.Line, 0, xvalues.Count, strategy.TakeProfit, strategy.TakeProfit, null, 1,
                Line.init(null, null, null, null, null, Color.fromRGB(77, 222, 4)), null,
                StyleParam.Layer.Below);

            var slSignalShape = Shape.init<int, int, decimal, decimal>(StyleParam.ShapeType.Line, 0, xvalues.Count, data.Notification.StopLoss, data.Notification.StopLoss, null, 0.5,
                Line.init(null, null, null, null, null, Color.fromRGB(105, 96, 79)), null,
                StyleParam.Layer.Below);
            var tp1SignalShape = Shape.init<int, int, decimal, decimal>(StyleParam.ShapeType.Line, 0, xvalues.Count, data.Notification.Target1, data.Notification.Target1, null, 0.5,
                Line.init(null, null, null, null, null, Color.fromRGB(105, 96, 79)), null,
                StyleParam.Layer.Below);
            var tp2SignalShape = Shape.init<int, int, decimal, decimal>(StyleParam.ShapeType.Line, 0, xvalues.Count, data.Notification.Target2, data.Notification.Target2, null, 0.5,
                Line.init(null, null, null, null, null, Color.fromRGB(105, 96, 79)), null,
                StyleParam.Layer.Below);
            var tp3SignalShape = Shape.init<int, int, decimal, decimal>(StyleParam.ShapeType.Line, 0, xvalues.Count, data.Notification.Target3, data.Notification.Target3, null, 0.5,
                Line.init(null, null, null, null, null, Color.fromRGB(105, 96, 79)), null,
                StyleParam.Layer.Below);
            var tp4SignalShape = Shape.init<int, int, decimal, decimal>(StyleParam.ShapeType.Line, 0, xvalues.Count, data.Notification.Target4, data.Notification.Target4, null, 0.5,
                Line.init(null, null, null, null, null, Color.fromRGB(105, 96, 79)), null,
                StyleParam.Layer.Below);
            var tp5SignalShape = Shape.init<int, int, decimal, decimal>(StyleParam.ShapeType.Line, 0, xvalues.Count, data.Notification.Target5, data.Notification.Target5, null, 0.5,
                Line.init(null, null, null, null, null, Color.fromRGB(105, 96, 79)), null,
                StyleParam.Layer.Below);

            var ymin = data.Data.Min() * 0.9M;
            var ymax = data.Data.Max() * 1.1M;

            var entryShape = Shape.init<int, int, decimal, decimal>(StyleParam.ShapeType.Line, strategy.StartIndex, strategy.StartIndex, ymin, ymax, null, 1,
                Line.init(null, null, null, null, null, Color.fromRGB(255, 200, 0)), null,
                StyleParam.Layer.Below);

            var exitShape = Shape.init<int, int, decimal, decimal>(StyleParam.ShapeType.Line, strategy.LastIndex, strategy.LastIndex, ymin, ymax, null, 1,
                Line.init(null, null, null, null, null, Color.fromRGB(255, 200, 0)), null,
                StyleParam.Layer.Below);

            chart.WithShapes(new List<Shape>
            {
                signalPriceLine,
                stopLossLine,
                takeProfitLine,
                slSignalShape,
                tp1SignalShape,
                tp2SignalShape,
                tp3SignalShape,
                tp4SignalShape,
                tp5SignalShape,
                entryShape,
                exitShape
            });

            var path = Path.Combine(FileSystemHelper.GetBaseDirectory(), "plot", data.FileName);
            var dir = Path.GetDirectoryName(path);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if(File.Exists(path))
            {
                File.Delete(path);
            }
            chart.SaveHtmlAs<object>(path);
            PatchHtml($"{path}.html");
        }

        /// <summary>
        ///     Sets scrollZoom to true - not possible by the wrapper
        /// </summary>
        /// <param name="path">path of the html</param>
        private void PatchHtml(string path)
        {
            if (!File.Exists(path))
                return;
            var txt = File.ReadAllText(path);
            txt = txt.Replace("var config = {\"responsive\":true};", "var config = {\"responsive\":true, \"scrollZoom\": true};");
            File.WriteAllText(path, txt);
        }
    }
}
using CoinLegsSignalBacktester.Model.CoinLegsSignalDataCollector.Model;
using Newtonsoft.Json;

namespace CoinLegsSignalBacktester.Backtest
{
    public class BacktestData
    {
        [JsonConverter(typeof(JsonStringConverter))]
        public Notification Notification { get; set; }

        public decimal LastPrice { get; set; }
        public List<decimal> Data { get; set; }
        public string FileName { get; set; }
        public DateTime Date { get; set; }
    }
}
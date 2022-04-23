using CoinLegsSignalBacktester.Model.CoinLegsSignalDataCollector.Model;
using Newtonsoft.Json;

namespace CoinLegsSignalBacktester.Backtest
{
    public class BacktestData
    {
        public int Version { get; set; } = 1;
        [JsonConverter(typeof(JsonBacktestDataNotificationConverter))]
        public Notification Notification { get; set; }

        public decimal LastPrice { get; set; }
        public List<decimal> Data { get; set; }
        public string FileName { get; set; }
        public DateTime Date { get; set; }

        [JsonConverter(typeof(JsonTicksDateTimeConverter))]
        public List<DateTime> Times { get; set; } = new();
    }
}
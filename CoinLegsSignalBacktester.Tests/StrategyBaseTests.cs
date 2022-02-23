using System.Collections.Generic;
using CoinLegsSignalBacktester.Backtest;
using CoinLegsSignalBacktester.Model.CoinLegsSignalDataCollector.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoinLegsSignalBacktester.Tests
{
    [TestClass]
    public class StrategyBaseTests
    {
        [TestMethod]
        public void Take_Profit_Long_Valid()
        {
            var data = new List<decimal>
            {
                0.05M,
                0.1M,
                0.2M,
                0.25M,
                0.4M,
                0.5M
            };
            var btData = new BacktestData
            {
                Data = data,
                LastPrice = 0.075M,
                Notification = new Notification
                {
                    SignalPrice = 0.075M,
                    StopLoss = 0.01M,
                    Target1 = 0.2M,
                    Target2 = 0.25M,
                    Signal = 1
                }
            };
            var btConfig = new BacktestConfig
            {
                Parameters = new List<BacktestParameter>
                {
                    new()
                    {
                        Key = "TakeProfit",
                        Value = btData.Notification.Target2
                    },
                    new()
                    {
                        Key = "StopLoss",
                        Value = btData.Notification.StopLoss
                    },
                    new()
                    {
                        Key = "UseStopLoss",
                        Value = true
                    },
                    new()
                    {
                        Key = "MarketBuy",
                        Value = true
                    }
                }
            };
            var strategy = new StrategyBaseImpl();
            var result = strategy.Backtest(btData, btConfig);
            Assert.IsTrue(result.ExitPrice == btData.Notification.Target2);
        }

        [TestMethod]
        public void Stop_Loss_Long_Valid()
        {
            var data = new List<decimal>
            {
                0.5M,
                0.4M,
                0.3M,
                0.2M,
                0.1M,
                0.05M
            };
            var btData = new BacktestData
            {
                Data = data,
                LastPrice = 0.075M,
                Notification = new Notification
                {
                    SignalPrice = 0.3M,
                    StopLoss = 0.1M,
                    Target1 = 0.6M,
                    Target2 = 0.7M,
                    Signal = 1
                }
            };
            var btConfig = new BacktestConfig
            {
                Parameters = new List<BacktestParameter>
                {
                    new()
                    {
                        Key = "TakeProfit",
                        Value = btData.Notification.Target1
                    },
                    new()
                    {
                        Key = "StopLoss",
                        Value = btData.Notification.StopLoss
                    },
                    new()
                    {
                        Key = "UseStopLoss",
                        Value = true
                    },
                    new()
                    {
                        Key = "MarketBuy",
                        Value = true
                    }
                }
            };
            var strategy = new StrategyBaseImpl();
            var result = strategy.Backtest(btData, btConfig);
            Assert.IsTrue(result.ExitPrice == btData.Notification.StopLoss);
            Assert.IsTrue(result.StopLossHit);
        }

        [TestMethod]
        public void Take_Profit_Short_Valid()
        {
            var data = new List<decimal>
            {
                0.5M,
                0.4M,
                0.3M,
                0.2M,
                0.1M,
                0.05M
            };
            var btData = new BacktestData
            {
                Data = data,
                LastPrice = 0.4M,
                Notification = new Notification
                {
                    SignalPrice = 0.4M,
                    StopLoss = 0.6M,
                    Target1 = 0.2M,
                    Target2 = 0.1M,
                    Signal = -1
                }
            };
            var btConfig = new BacktestConfig
            {
                Parameters = new List<BacktestParameter>
                {
                    new()
                    {
                        Key = "TakeProfit",
                        Value = btData.Notification.Target2
                    },
                    new()
                    {
                        Key = "StopLoss",
                        Value = btData.Notification.StopLoss
                    },
                    new()
                    {
                        Key = "UseStopLoss",
                        Value = true
                    },
                    new()
                    {
                        Key = "MarketBuy",
                        Value = true
                    }
                }
            };
            var strategy = new StrategyBaseImpl();
            var result = strategy.Backtest(btData, btConfig);
            Assert.IsTrue(result.ExitPrice == btData.Notification.Target2);
        }

        [TestMethod]
        public void Stop_Loss_Short_Valid()
        {
            var data = new List<decimal>
            {
                0.05M,
                0.1M,
                0.2M,
                0.25M,
                0.4M,
                0.5M
            };
            var btData = new BacktestData
            {
                Data = data,
                LastPrice = 0.1M,
                Notification = new Notification
                {
                    SignalPrice = 0.1M,
                    StopLoss = 0.1M,
                    Target1 = 0.01M,
                    Target2 = 0.001M,
                    Signal = -1
                }
            };
            var btConfig = new BacktestConfig
            {
                Parameters = new List<BacktestParameter>
                {
                    new()
                    {
                        Key = "TakeProfit",
                        Value = btData.Notification.Target1
                    },
                    new()
                    {
                        Key = "StopLoss",
                        Value = btData.Notification.StopLoss
                    },
                    new()
                    {
                        Key = "UseStopLoss",
                        Value = true
                    },
                    new()
                    {
                        Key = "MarketBuy",
                        Value = true
                    }
                }
            };
            var strategy = new StrategyBaseImpl();
            var result = strategy.Backtest(btData, btConfig);
            Assert.IsTrue(result.ExitPrice == btData.Notification.StopLoss);
            Assert.IsTrue(result.StopLossHit);
        }

        [TestMethod]
        public void Test_Market_Buy()
        {
            var data = new List<decimal>
            {
                0.05M,
                0.1M,
                0.2M,
                0.25M,
                0.4M,
                0.5M
            };
            var btData = new BacktestData
            {
                Data = data,
                LastPrice = 0.1M,
                Notification = new Notification
                {
                    SignalPrice = 0.3M,
                    StopLoss = 0.1M,
                    Target1 = 0.01M,
                    Target2 = 0.001M,
                    Signal = -1
                }
            };
            var btConfig = new BacktestConfig
            {
                Parameters = new List<BacktestParameter>
                {
                    new()
                    {
                        Key = "TakeProfit",
                        Value = btData.Notification.Target1
                    },
                    new()
                    {
                        Key = "StopLoss",
                        Value = btData.Notification.StopLoss
                    },
                    new()
                    {
                        Key = "UseStopLoss",
                        Value = true
                    },
                    new()
                    {
                        Key = "MarketBuy",
                        Value = true
                    }
                }
            };
            var strategy = new StrategyBaseImpl();
            var result = strategy.Backtest(btData, btConfig);
            Assert.IsTrue(result.EntryPrice == btData.LastPrice);
        }

        [TestMethod]
        public void Test_Limit_Buy()
        {
            var data = new List<decimal>
            {
                0.2M,
                0.1M,
                0.15M,
                0.25M,
                0.4M,
                0.5M
            };
            var btData = new BacktestData
            {
                Data = data,
                LastPrice = 0.2M,
                Notification = new Notification
                {
                    SignalPrice = 0.15M,
                    StopLoss = 0.01M,
                    Target1 = 0.4M,
                    Target2 = 0.5M,
                    Signal = 1
                }
            };
            var btConfig = new BacktestConfig
            {
                Parameters = new List<BacktestParameter>
                {
                    new()
                    {
                        Key = "TakeProfit",
                        Value = btData.Notification.Target1
                    },
                    new()
                    {
                        Key = "StopLoss",
                        Value = btData.Notification.StopLoss
                    },
                    new()
                    {
                        Key = "UseStopLoss",
                        Value = true
                    },
                    new()
                    {
                        Key = "MarketBuy",
                        Value = false
                    }
                }
            };
            var strategy = new StrategyBaseImpl();
            var result = strategy.Backtest(btData, btConfig);
            Assert.IsTrue(result.EntryPrice == btData.Notification.SignalPrice);
        }

        [TestMethod]
        public void Test_Invalid_Backtest_No_Buy()
        {
            var data = new List<decimal>
            {
                0.2M,
                0.1M,
                0.15M,
                0.25M,
                0.4M,
                0.5M
            };
            var btData = new BacktestData
            {
                Data = data,
                LastPrice = 0.2M,
                Notification = new Notification
                {
                    SignalPrice = 0.05M,
                    StopLoss = 0.01M,
                    Target1 = 0.4M,
                    Target2 = 0.5M,
                    Signal = 1
                }
            };
            var btConfig = new BacktestConfig
            {
                Parameters = new List<BacktestParameter>
                {
                    new()
                    {
                        Key = "TakeProfit",
                        Value = btData.Notification.Target1
                    },
                    new()
                    {
                        Key = "StopLoss",
                        Value = btData.Notification.StopLoss
                    },
                    new()
                    {
                        Key = "UseStopLoss",
                        Value = true
                    },
                    new()
                    {
                        Key = "MarketBuy",
                        Value = false
                    }
                }
            };
            var strategy = new StrategyBaseImpl();
            var result = strategy.Backtest(btData, btConfig);
            Assert.IsTrue(result.State == BackTestResultState.Invalid);
        }
    }
}
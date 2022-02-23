using CoinLegsSignalBacktester.Strategy;

namespace CoinLegsSignalBacktester
{
    internal static class StrategyHelper
    {
        public static StrategyBase GetStrategyByName(string strategyName)
        {
            if (strategyName == "MarketPlaceFixedTakeProfitStrategy")
            {
                return new MarketPlaceFixedTakeProfitStrategy();
            }
            if (strategyName == "BlackFishMoveTakeProfitM2Strategy")
            {
                return new BlackFishMoveTakeProfitM2Strategy();
            }
            if (strategyName == "MarketPlaceTrailingStopLossStrategy")
            {
                return new MarketPlaceTrailingStopLossStrategy();
            }

            return null;
        }
    }
}
using CoinLegsSignalBacktester.Strategy;

namespace CoinLegsSignalBacktester
{
    internal static class StrategyHelper
    {
        public static StrategyBase GetStrategyByName(string strategyName)
        {
            return strategyName switch
            {
                "MarketPlaceFixedTakeProfitStrategy" => new MarketPlaceFixedTakeProfitStrategy(),
                "BlackFishMoveTakeProfitM2Strategy" => new BlackFishMoveTakeProfitM2Strategy(),
                "MarketPlaceTrailingStopLossStrategy" => new MarketPlaceTrailingStopLossStrategy(),
                "MarketPlacePartialTakeProfitTrailingStrategy" => new MarketPlacePartialTakeProfitTrailingStrategy(),
                _ => null
            };
        }
    }
}
namespace CoinLegsSignalBacktester.Model
{
    public struct CoolDownPeriodConfig
    {
        public int CoolDownHours { get; set; }
        public int PositionCount { get; set; }
        public decimal MaxDrawdown { get; set; }
    }
}

namespace CoinLegsSignalBacktester.Optimize
{
    internal class ParameterRandomizer
    {
        private readonly Random _rnd = new();

        public object GetNextValue(OptimizationParameter parameter)
        {
            if (parameter.Min is double minDouble)
            {
                var max = (double)parameter.Max;
                var diff = max - minDouble;
                var factor = _rnd.NextDouble();
                return minDouble + factor * diff;
            }

            if (parameter.Min is bool)
            {
                var value = _rnd.Next(0, 1);
                return value == 0 ? parameter.Min : parameter.Max;
            }

            if (parameter.Min is long minLong)
            {
                var maxInt = unchecked((int)(long)parameter.Max);
                var minInt = unchecked((int)minLong);
                return _rnd.Next(minInt, maxInt);
            }

            throw new NotImplementedException($"type {parameter.Min.GetType()} not implemented for optimization");
        }
    }
}
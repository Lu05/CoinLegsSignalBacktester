using Newtonsoft.Json.Linq;

namespace CoinLegsSignalBacktester.Optimize
{
    internal class ParameterRandomizer
    {
        private readonly Random _rnd = new();

        public object GetNextValue(OptimizationParameter parameter)
        {
            if (parameter.Key == "Profits")
            {
                var values = new List<double>();
                double total = (double)parameter.Max;
                var array = new JArray();
                for (int i = 0; i < 5; i++)
                {
                    var next = GetRandomNumber(0.0, total);
                    if (i == 4)
                    {
                        next = 1 - values.Sum();
                    }
                    values.Add(next);
                    total -= next;
                    array.Add(new JObject
                    {
                        new JProperty("Index", i+1),
                        new JProperty("Amount", next)
                    });
                }

                return array;
            }
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

        private double GetRandomNumber(double minimum, double maximum)
        { 
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
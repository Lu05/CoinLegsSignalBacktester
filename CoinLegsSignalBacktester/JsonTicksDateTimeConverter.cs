using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CoinLegsSignalBacktester
{
    internal class JsonTicksDateTimeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new List<DateTime>();
            if (reader.TokenType == JsonToken.StartArray)
            {
                var tokenType = reader.TokenType;
                while (tokenType != JsonToken.EndArray)
                {
                    reader.Read();
                    tokenType = reader.TokenType;
                    if(tokenType == JsonToken.EndArray)
                        break;
                    result.Add(new DateTime((long)reader.Value, DateTimeKind.Utc));
                }
            }

            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }
    }
}
using Newtonsoft.Json;

namespace CoinLegsSignalBacktester;

public class JsonStringConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return string.Empty;

        var obj = serializer.Deserialize(reader, typeof(string));
        return JsonConvert.DeserializeObject((string)obj ?? string.Empty, objectType);
    }

    public override bool CanConvert(Type objectType)
    {
        return false;
    }
}
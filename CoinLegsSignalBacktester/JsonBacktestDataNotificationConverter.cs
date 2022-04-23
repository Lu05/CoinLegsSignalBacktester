using CoinLegsSignalBacktester.Model.CoinLegsSignalDataCollector.Model;
using Newtonsoft.Json;

namespace CoinLegsSignalBacktester;

public class JsonBacktestDataNotificationConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return string.Empty;
        //Notification is a direct Json Value
        if (reader.TokenType == JsonToken.StartObject) return serializer.Deserialize<Notification>(reader);
        //Notification is a string Json Value
        var obj = serializer.Deserialize(reader, typeof(string));
        return JsonConvert.DeserializeObject((string)obj ?? string.Empty, objectType);
    }

    public override bool CanConvert(Type objectType)
    {
        return false;
    }
}
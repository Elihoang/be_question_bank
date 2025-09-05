using Newtonsoft.Json;
using System;
using BEQuestionBank.Domain.Enums;

namespace BEQuestionBank.Shared.Helpers
{
    public class IntToEnumCloConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(EnumCLO);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                return (EnumCLO)Convert.ToInt32(reader.Value);
            }

            if (reader.TokenType == JsonToken.String)
            {
                return Enum.Parse<EnumCLO>(reader.Value.ToString());
            }

            throw new JsonSerializationException("Invalid CLO value");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((int)(EnumCLO)value);
        }
    }
}
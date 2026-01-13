using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Codeworx.Units
{
    public class UnitJsonConverter<TUnit> : JsonConverter<TUnit>
      where TUnit : IUnitBase
    {
        private Func<string, TUnit> _parser;

        public UnitJsonConverter(Func<string, TUnit> parser)
        {
            _parser = parser;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(TUnit).IsAssignableFrom(typeToConvert);
        }

        public override TUnit Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return _parser(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, TUnit value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
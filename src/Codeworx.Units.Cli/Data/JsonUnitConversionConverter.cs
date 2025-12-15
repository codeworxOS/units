using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Codeworx.Units.Cli.Data
{
    internal class JsonUnitConversionConverter : JsonConverter<JsonUnitConversion>
    {
        public override JsonUnitConversion? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var node = JsonNode.Parse(ref reader);

            var result = new JsonUnitConversion();
            if (node is JsonObject obj)
            {
                if (obj.TryGetPropertyValue("Factor", out var factor) && factor != null)
                {
                    result.Factor = factor.GetValue<decimal>();
                }
                if (obj.TryGetPropertyValue("Divisor", out var divisor) && divisor != null)
                {
                    result.Divisor = divisor.GetValue<decimal>();
                }
                if (obj.TryGetPropertyValue("Offset", out var offset) && offset != null)
                {
                    result.Offset = offset.GetValue<decimal>();
                }
            }
            else if(node is JsonValue val)
            {
                result.Factor = val.GetValue<decimal>();
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, JsonUnitConversion value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
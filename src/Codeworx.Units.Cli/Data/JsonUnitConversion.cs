using System.Text.Json.Serialization;

namespace Codeworx.Units.Cli.Data
{
    [JsonConverter(typeof(JsonUnitConversionConverter))]
    public class JsonUnitConversion
    {
        public decimal Divisor { get; set; } = 1;

        public decimal Factor { get; set; } = 1;

        public decimal Offset { get; set; } = 0;
    }
}
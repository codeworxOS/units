using System.Collections.Generic;
using System.Text.Json.Serialization;
using Codeworx.Units;

namespace Codeworx.Units.Cli.Data
{
    public class JsonUnit
    {
        [JsonIgnore]
        public string Name { get; set; } = null!;

        public required string Symbol { get; set; }

        public string? Key { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UnitSystem? System { get; set; }

        public decimal Divisor { get; set; } = 1;

        public decimal Factor { get; set; } = 1;

        public decimal Offset { get; set; } = 0;

        public Dictionary<string, JsonUnitConversion> Conversions { get; set; } = new Dictionary<string, JsonUnitConversion>();
    }
}
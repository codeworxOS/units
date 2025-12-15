using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Codeworx.Units.Cli.Data
{
    public class JsonDimension
    {
        [JsonIgnore]
        public string Name { get; set; } = null!;

        public required string SIUnit { get; set; }

        public required string BaseUnit { get; set; }

        public required string ImperialDefault { get; set; }

        public required string MetricDefault { get; set; }

        public Dictionary<string, JsonUnit> Units { get; set; } = new Dictionary<string, JsonUnit>();
    }
}

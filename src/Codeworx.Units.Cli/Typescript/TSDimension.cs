
using System.Collections.Generic;
using System.Linq;

namespace Codeworx.Units.Cli.Typescript
{

    public class TSDimension
    {
        public required string DimensionName { get; set; }

        public required string DefaultUnit { get; set; }

        public required string DefaultImperial { get; set; }

        public required string DefaultMetric { get; set; }

        public required string SIUnit { get; set; }

        public required List<TSUnit> Units { get; set; }

        public string UnitNames => string.Join(" | ", Units.Select(d => "'" + d.UnitName + "'"));
    }
}
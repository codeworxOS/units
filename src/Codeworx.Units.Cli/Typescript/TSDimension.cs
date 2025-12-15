
using System.Collections.Generic;

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
    }
}
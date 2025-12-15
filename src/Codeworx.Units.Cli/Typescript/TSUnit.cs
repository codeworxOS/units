
using System.Collections.Generic;

namespace Codeworx.Units.Cli.Typescript
{
    public class TSUnit
    {
        public required string DimensionName { get; set; }

        public required string UnitName { get; set; }

        public required string Symbol { get; set; }

        public required List<TSConversion> Conversion { get; set; }
    }
}
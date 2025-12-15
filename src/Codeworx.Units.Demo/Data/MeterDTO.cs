using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Units.Defaults.DistanceDimension;

namespace Codeworx.Units.Demo.Data
{
    public class MeterDTO
    {
        public int? Id { get; set; }

        public required Meter RequiredMeter { get; set; }

        public Meter? OptionalMeter { get; set; }
    }
}

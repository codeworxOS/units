using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Units.Defaults;

namespace Codeworx.Units.Demo.Data
{
    public class DistanceDTO
    {
        public int? Id { get; set; }

        public required IDistance RequiredDistance { get; set; }

        public IDistance? OptionalDistance { get; set; }
    }
}

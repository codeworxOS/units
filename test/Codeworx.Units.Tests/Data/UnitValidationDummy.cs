using Codeworx.Units.Defaults;
using Codeworx.Units.Defaults.DistanceDimension;
using Codeworx.Units.EntityFrameworkCore.Validation;

namespace Codeworx.Units.Tests.Data
{
    internal class UnitValidationDummy : ValidationBase
    {
        [DimensionRange<IDistance>("5 m", "0.010 km")]
        public required Meter Distance { get; set; }
    }
}

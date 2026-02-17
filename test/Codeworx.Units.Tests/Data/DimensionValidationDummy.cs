using Codeworx.Units.Defaults;
using Codeworx.Units.EntityFrameworkCore.Validation;

namespace Codeworx.Units.Tests.Data
{
    internal class DimensionValidationDummy : ValidationBase
    {
        [DimensionRange<IDistance>("5 m", "0.010 km")]
        public required IDistance Distance { get; set; }
    }
}

using Codeworx.Units.Defaults;
using Codeworx.Units.EntityFrameworkCore;

namespace Units.Demo.Database.Model
{
    public class DistanceDimensionModel
    {
        public int Id { get; set; }

        public required DimensionValue<IDistance> RequiredDistance { get; set; }

        public DimensionValue<IDistance>? OptionalDistance { get; set; }
    }
}
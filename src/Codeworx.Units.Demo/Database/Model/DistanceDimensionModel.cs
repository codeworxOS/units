using Codeworx.Units.Defaults;
using Codeworx.Units.EntityFrameworkCore.Entities;

namespace Units.Demo.Database.Model
{
    public class DistanceDimensionModel
    {
        public int Id { get; set; }

        public required DimensionValue<IDistance> RequiredDistance { get; init; }

        public required NullableDimensionValue<IDistance> OptionalDistance { get; init; }
    }
}
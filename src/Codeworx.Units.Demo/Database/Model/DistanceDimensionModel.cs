using Codeworx.Units.Defaults;
using Codeworx.Units.EntityFrameworkCore.Entities;

namespace Units.Demo.Database.Model
{
    public class DistanceDimensionModel
    {
        public int Id { get; set; }

        public required DimensionValue<IDistance> RequiredDistance { get; set; }

        public required NullableDimensionValue<IDistance> OptionalDistance { get; set; }
    }
}
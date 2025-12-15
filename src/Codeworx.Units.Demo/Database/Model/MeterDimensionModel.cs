using Codeworx.Units.Defaults.DistanceDimension;

namespace Units.Demo.Database.Model
{
    public class MeterDimensionModel
    {
        public int Id { get; set; }

        public Meter RequiredMeter { get; set; }

        public Meter? OptionalMeter { get; set; }
    }
}
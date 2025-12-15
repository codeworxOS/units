using Codeworx.Units.Defaults.DistanceDimension;

namespace Units.Demo.Database.Model
{
    public class MeterDimensionTest
    {
        public int Id { get; set; }

        public Meter Meter { get; set; }

        public Meter? MeterNullable { get; set; }
    }
}
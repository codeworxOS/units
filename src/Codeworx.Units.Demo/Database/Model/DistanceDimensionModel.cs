using Codeworx.Units.Defaults;
using Codeworx.Units.EntityFrameworkCore;

namespace Units.Demo.Database.Model
{
    public class DistanceDimensionModel
    {
        public int Id { get; set; }

        public DimensionValue<IDistance>? Distance { get; set; }
    }
}
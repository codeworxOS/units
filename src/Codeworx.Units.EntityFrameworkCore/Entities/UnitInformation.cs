using System.ComponentModel.DataAnnotations;

namespace Codeworx.Units.EntityFrameworkCore.Entities
{
    public class UnitInformation
    {
        [MaxLength(32)]
        public required string Id { get; set; }

        [MaxLength(10)]
        public required string Symbol { get; set; }

        public required decimal ConversionOffset { get; set; }

        public required decimal ConversionDivisor { get; set; }

        public required decimal ConversionFactor { get; set; }
    }
}

namespace Codeworx.Units.EntityFrameworkCore.Entities
{
    public class UnitInformation
    {
        public string Id { get; set; }

        public decimal ConversionOffset { get; set; }

        public decimal ConversionDivisor { get; set; }

        public decimal ConversionFactor { get; set; }
    }
}


namespace Codeworx.Units.Cli.Typescript
{
    public class TSConversion
    {
        public required string DimensionName { get; set; }

        public required string ConversionUnitName { get; set; }

        public string? Conversion { get; set; }

        public bool HasConversion => !string.IsNullOrWhiteSpace(Conversion);
    }
}
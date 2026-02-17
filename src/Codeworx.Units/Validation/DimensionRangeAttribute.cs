using System.ComponentModel.DataAnnotations;

namespace Codeworx.Units.EntityFrameworkCore.Validation
{
    public class DimensionRangeAttribute<TDimension> : RangeAttribute
        where TDimension : IUnitBase
    {
        public DimensionRangeAttribute(string minimum, string maximum)
            : base(typeof(TDimension), minimum, maximum)
        {
        }

        public override bool IsValid(object? value)
        {
            var newValue = TryGetDimensionValue(value);

            return base.IsValid(newValue);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var newValue = TryGetDimensionValue(value);

            return base.IsValid(newValue, validationContext);
        }

        private object? TryGetDimensionValue(object? value)
        {
            if (value is IDimensionValue<TDimension> dimensionValue)
            {
                return dimensionValue.GetDimension();
            }
            else if (value is INullableDimensionValue<TDimension> nullableDimensionValue)
            {
                return nullableDimensionValue.GetDimension();
            }

            return value;
        }
    }
}

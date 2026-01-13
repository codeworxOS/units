using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Units.EntityFrameworkCore.Entities
{
    [Owned]
    [DebuggerDisplay("UnitValue = {GetDimension()?.ToString()}")]
    public class NullableDimensionValue<T>
        where T : IUnitBase
    {
        public decimal? Value { get; set; }

        [MaxLength(32)]
        public string? UnitId { get; set; }

        public UnitInformation? Unit { get; set; }

        public static NullableDimensionValue<T> GetEntity(T? entry)
        {
            return new NullableDimensionValue<T> { Value = entry?.BaseValue, UnitId = entry?.Key };
        }

        public T? GetDimension()
        {
            if (!Value.HasValue || string.IsNullOrEmpty(UnitId))
            {
                return default;
            }

            return DimensionParser.Get<T>(UnitId, Value.Value);
        }
    }
}
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Codeworx.Units.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Units.EntityFrameworkCore.Entities
{
    [Owned]
    [DebuggerDisplay("UnitValue = {GetDimension()?.ToString()}")]
    public class NullableDimensionValue<T>
        where T : IUnitBase
    {
        public decimal? Value { get; set; }

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
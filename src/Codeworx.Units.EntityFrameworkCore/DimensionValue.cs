using Codeworx.Units.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Units.EntityFrameworkCore
{
    [Owned]
    public class DimensionValue<T>
        where T : IUnitBase
    {
        public required decimal Value { get; set; }

        public required string UnitKey { get; set; }

        public static DimensionValue<T>? GetEntity(T? entry)
        {
            if (entry == null)
                return null;

            return new DimensionValue<T> { Value = entry.BaseValue, UnitKey = entry.Key };
        }

        public T GetDimension()
        {
            return DimensionParser.Get<T>(UnitKey, Value);
        }
    }
}
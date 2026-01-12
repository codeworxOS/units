using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Codeworx.Units.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Units.EntityFrameworkCore.Entities
{
    [Owned]
    [DebuggerDisplay("UnitValue = {GetDimension().ToString()}")]
    public class DimensionValue<T>
        where T : IUnitBase
    {
        public required decimal Value { get; set; }

        [MaxLength(32)]
        public required string UnitId { get; set; }

        public UnitInformation? Unit { get; set; }

        public static DimensionValue<T> GetEntity(T entry)
        {
            return new DimensionValue<T> { Value = entry.BaseValue, UnitId = entry.Key };
        }

        public T GetDimension()
        {
            return DimensionParser.Get<T>(UnitId, Value);
        }
    }
}
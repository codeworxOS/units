using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Codeworx.Units.EntityFrameworkCore
{
    internal class UnitEntityConverter<T> : ValueConverter<T, decimal>
      where T : IUnitBase
    {
        public UnitEntityConverter()
          : base(GetToDecimalExpression(), GetToUnitExpression(), new ConverterMappingHints(precision: 14, scale: 4))
        {
        }

        private static Expression<Func<T, decimal>> GetToDecimalExpression()
        {
            return d => d.Value;
        }

        private static Expression<Func<decimal, T>> GetToUnitExpression()
        {
            var param = Expression.Parameter(typeof(decimal));

            var body = Expression.New(typeof(T).GetConstructors()[0], param);

            var exp = Expression.Lambda<Func<decimal, T>>(body, param);

            return exp;
        }
    }
}

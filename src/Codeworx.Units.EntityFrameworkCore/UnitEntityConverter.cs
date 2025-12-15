using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Codeworx.Units.Primitives;
using System;

namespace Codeworx.Units.EntityFrameworkCore
{
    internal class UnitEntityConverter<T> : ValueConverter<T, decimal>
      where T : IUnitBase
    {
        public UnitEntityConverter()
          : base(GetToDecimalExpression(), GetToUnitExpression())
        {
        }

        private static Expression<Func<T, decimal>> GetToDecimalExpression()
        {
            return d => d.BaseValue;
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

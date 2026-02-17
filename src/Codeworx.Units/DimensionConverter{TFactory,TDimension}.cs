using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;

namespace Codeworx.Units
{
    public class DimensionTypeConverter<TDimension> : TypeConverter
        where TDimension : IUnitBase
    {
        private static Func<string, TDimension> _dimensionParser;

        static DimensionTypeConverter()
        {
            var method = typeof(TDimension).GetMethod("Parse", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, [typeof(string)]);

            if (method == null)
            {
                throw new InvalidOperationException("Expected static \"Parse(string)\" method!");
            }

            var param = Expression.Parameter(typeof(string), "str");
            var body = Expression.Call(null, method, param);
            var expression = Expression.Lambda<Func<string, TDimension>>(body, param);

            _dimensionParser = expression.Compile();
        }

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return false;
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            if (destinationType == typeof(TDimension))
            {
                return true;
            }

            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string str)
            {
                return _dimensionParser(str);
            }

            return value;
        }
    }
}

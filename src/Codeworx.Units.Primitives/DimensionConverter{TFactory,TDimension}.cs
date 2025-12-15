using System;
using System.ComponentModel;
using System.Globalization;

namespace Codeworx.Units.Primitives
{
    public class DimensionTypeConverter<TDimension> : TypeConverter
        where TDimension : IUnitBase
    {
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
                ////var parser = new TFactory();
                ////return parser.ParseFromFullString(str);
            }

            return value;
        }
    }
}

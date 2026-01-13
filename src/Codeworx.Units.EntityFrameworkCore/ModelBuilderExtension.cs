using System.Linq;
using System.Reflection;
using Codeworx.Units;
using Codeworx.Units.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore
{
    public static partial class ModelBuilderExtension
    {
        public static void AddUnitConversionConvention(this ModelConfigurationBuilder configurationBuilder, Assembly unitAssembly)
        {
            var types = unitAssembly.GetTypes();
            var unitBaseType = typeof(IUnitBase);

            var units = types.Where(d => d.IsValueType && unitBaseType.IsAssignableFrom(d)).ToList();

            foreach (var type in units)
            {
                configurationBuilder.Properties(type).HaveConversion(typeof(UnitEntityConverter<>).MakeGenericType(type));
            }
        }
    }
}

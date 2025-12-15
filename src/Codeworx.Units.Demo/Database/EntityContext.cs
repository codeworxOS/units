using Codeworx.Units.Defaults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Units.Demo.Database.Model;

namespace Units.Demo.Database
{
    public class EntityContext : DbContext
    {
        public EntityContext(DbContextOptions options) : base(options)
        {
        }

        protected EntityContext()
        {
        }

        public DbSet<DistanceDimensionModel> DistanceDimensionTest { get; set; }

        public DbSet<MeterDimensionModel> MeterDimensionTest { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            configurationBuilder.AddUnitConversionConvention(typeof(IDistance).Assembly);
        }
    }
}

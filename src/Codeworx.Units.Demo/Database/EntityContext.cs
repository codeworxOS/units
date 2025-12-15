using Codeworx.Units.Defaults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Units.Demo.Database.Model;

namespace Units.Demo.Database
{
    public class EntityContext : DbContext
    {
        public DbSet<DistanceDimensionModel> DistanceDimensionTest { get; set; }

        public DbSet<MeterDimensionTest> MeterDimensionTest { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=UnitTestData;Trusted_Connection=True;");
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            configurationBuilder.AddUnitConversionConvention(typeof(IDistance).Assembly);
        }
    }
}

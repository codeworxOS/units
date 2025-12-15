using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Units.Defaults;
using Codeworx.Units.Defaults.DistanceDimension;
using Codeworx.Units.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Units.Demo.Database;
using Units.Demo.Database.Model;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var context = new EntityContext();
        await context.Database.EnsureDeletedAsync();

        await context.Database.EnsureCreatedAsync();

        IDistance? tmpVal = null;
        IDistance? tmpVal2 = new Yard(5);

        context.DistanceDimensionTest.Add(new DistanceDimensionModel { Distance = DimensionValue<IDistance>.GetEntity(new Meter(10)) });
        context.DistanceDimensionTest.Add(new DistanceDimensionModel { Distance = DimensionValue<IDistance>.GetEntity(null) });
        context.DistanceDimensionTest.Add(new DistanceDimensionModel { Distance = DimensionValue<IDistance>.GetEntity(new Kilometer(3)) });
        context.DistanceDimensionTest.Add(new DistanceDimensionModel { Distance = DimensionValue<IDistance>.GetEntity(tmpVal) });
        context.DistanceDimensionTest.Add(new DistanceDimensionModel { Distance = DimensionValue<IDistance>.GetEntity(tmpVal2) });

        context.MeterDimensionTest.Add(new MeterDimensionTest { Meter = 10, MeterNullable = null });
        context.MeterDimensionTest.Add(new MeterDimensionTest { Meter = 10, MeterNullable = 5 });

        await context.SaveChangesAsync();

        var tmp = context.DistanceDimensionTest.AsNoTracking().Select(d => d.Distance != null ? d.Distance.GetDimension() : null).ToList();

        var tmp2 = context.MeterDimensionTest.AsNoTracking().ToList();

        Console.WriteLine();
    }
}
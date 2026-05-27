using System.Linq;
using System.Threading.Tasks;
using Codeworx.Units.Defaults;
using Codeworx.Units.Defaults.DistanceDimension;
using Codeworx.Units.Demo.Data;
using Codeworx.Units.EntityFrameworkCore;
using Codeworx.Units.EntityFrameworkCore.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Units.Demo.Database;
using Units.Demo.Database.Model;


internal class Program
{
    private static async Task Main(string[] args)
    {
        //var test = await context.DistanceDimensionTest.OrderBy(d => (d.RequiredDistance.Value + d.RequiredDistance.Unit!.ConversionOffset) * d.RequiredDistance.Unit!.ConversionFactor / d.RequiredDistance.Unit!.ConversionDivisor)
        //    .ToListAsync();

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<EntityContext>(opt => opt.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=UnitTestData;Trusted_Connection=True;").AddDimensionQueryReplacement());

        builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.AddUnitConverters());

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApiDocument();

        var app = builder.Build();

        app.MapPost("/RecreateDB", async ([FromServices] EntityContext context) =>
        {
            await context.Database.EnsureDeletedAsync();

            await context.Database.EnsureCreatedAsync();

            foreach (var item in UnitExtensions.GetEntityInformation())
            {
                context.Set<UnitInformation>().Add(new UnitInformation { Id = item.Key, Symbol = item.Symbol, ConversionDivisor = item.Divisior, ConversionFactor = item.Factor, ConversionOffset = item.Offset });
            }

            await context.SaveChangesAsync();
        });

        app.MapPost("/AddDistance", async ([FromServices] EntityContext context, [FromBody] DistanceDTO data) =>
        {
            DistanceDimensionModel entry;
            if (data.Id != null)
            {
                entry = await context.DistanceDimensionTest.FirstAsync(d => d.Id == data.Id);
                entry.RequiredDistance.Set(data.RequiredDistance);
                entry.OptionalDistance.Set(data.OptionalDistance);
            }
            else
            {
                entry = new DistanceDimensionModel
                {
                    RequiredDistance = DimensionValue<IDistance>.GetEntity(data.RequiredDistance),
                    OptionalDistance = NullableDimensionValue<IDistance>.GetEntity(data.OptionalDistance),
                };
                context.DistanceDimensionTest.Add(entry);
            }

            await context.SaveChangesAsync();

            return await GetDistance(context, entry.Id);
        });

        app.MapPost("/AddMeter", async ([FromServices] EntityContext context, [FromBody] MeterDTO data) =>
        {
            MeterDimensionModel entry;
            if (data.Id != null)
            {
                entry = await context.MeterDimensionTest.FirstAsync(d => d.Id == data.Id);
                entry.RequiredMeter = data.RequiredMeter;
                entry.OptionalMeter = data.OptionalMeter;
            }
            else
            {
                entry = new MeterDimensionModel
                {
                    RequiredMeter = data.RequiredMeter,
                    OptionalMeter = data.OptionalMeter,
                };
                context.MeterDimensionTest.Add(entry);
            }

            await context.SaveChangesAsync();

            return await GetMeter(context, entry.Id);
        });

        app.MapGet("/QueryDistance", async ([FromServices] EntityContext context, int Id) =>
        {
            var tmp = await GetDistance(context, Id);
            return tmp;
        });

        app.MapGet("/QueryMeter", async ([FromServices] EntityContext context, int Id) =>
        {
            var tmp = await GetMeter(context, Id);
            return tmp;
        });

        app.UseOpenApi();
        app.UseSwaggerUi();

        await app.RunAsync();
    }

    private static async Task<MeterDTO?> GetMeter(EntityContext context, int Id)
    {
        var entryQry = context.MeterDimensionTest.Where(d => d.Id == Id).Select(d => new MeterDTO
        {
            Id = d.Id,
            RequiredMeter = d.RequiredMeter,
            OptionalMeter = d.OptionalMeter,
        })
            .OrderBy(p => p.RequiredMeter)
            .ThenBy(p => p.OptionalMeter);

        return await entryQry.FirstOrDefaultAsync();
    }

    private static async Task<DistanceDTO?> GetDistance(EntityContext context, int Id)
    {
        var entryQry = context.DistanceDimensionTest.Where(d => d.Id == Id).Select(d => new DistanceDTO
        {
            Id = d.Id,
            RequiredDistance = d.RequiredDistance.GetDimension(),
            OptionalDistance = d.OptionalDistance.GetDimension(),
            ////RequiredDistance = new Distance
            ////{
            ////    Symbol = d.RequiredDistance.Unit!.Symbol,
            ////    Value = d.RequiredDistance.Value,
            ////    BaseValue = d.RequiredDistance.Value * d.RequiredDistance.Unit.ConversionFactor / d.RequiredDistance.Unit.ConversionDivisor + d.RequiredDistance.Unit.ConversionOffset
            ////},
            ////OptionalDistance = d.OptionalDistance.Value != null ? new Distance
            ////{
            ////    Symbol = d.OptionalDistance.Unit!.Symbol,
            ////    Value = d.OptionalDistance.Value.Value,
            ////    BaseValue = d.OptionalDistance.Value.Value * d.OptionalDistance.Unit.ConversionFactor / d.OptionalDistance.Unit.ConversionDivisor + d.OptionalDistance.Unit.ConversionOffset
            ////} : null,
        })
            .Where(p => p.RequiredDistance >= new Meter(1) && p.RequiredDistance <= new Feet(9))
            .OrderBy(p => p.RequiredDistance)
            .ThenBy(p => p.OptionalDistance);

        return await entryQry.FirstOrDefaultAsync();
    }
}
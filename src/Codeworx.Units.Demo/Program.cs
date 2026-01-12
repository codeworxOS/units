using System.Linq;
using System.Threading.Tasks;
using Codeworx.Units.Defaults;
using Codeworx.Units.Demo.Data;
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

        builder.Services.AddDbContext<EntityContext>(opt => opt.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=UnitTestData;Trusted_Connection=True;"));

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
            var entry = new DistanceDimensionModel
            {
                RequiredDistance = DimensionValue<IDistance>.GetEntity(data.RequiredDistance),
                OptionalDistance = NullableDimensionValue<IDistance>.GetEntity(data.OptionalDistance),
            };
            context.DistanceDimensionTest.Add(entry);

            await context.SaveChangesAsync();

            return await GetDistance(context, entry.Id);
        });

        app.MapPost("/AddMeter", async ([FromServices] EntityContext context, [FromBody] MeterDTO data) =>
        {
            var entry = new MeterDimensionModel
            {
                RequiredMeter = data.RequiredMeter,
                OptionalMeter = data.OptionalMeter,
            };
            context.MeterDimensionTest.Add(entry);

            await context.SaveChangesAsync();

            return await GetMeter(context, entry.Id);
        });

        app.MapGet("/QueryDistance", async ([FromServices] EntityContext context, int Id) =>
        {
            return await GetDistance(context, Id);
        });

        app.MapGet("/QueryMeter", async ([FromServices] EntityContext context, int Id) =>
        {
            return await GetMeter(context, Id);
        });

        app.UseOpenApi();
        app.UseSwaggerUi();

        await app.RunAsync();
    }

    private static async Task<MeterDTO?> GetMeter(EntityContext context, int Id)
    {
        var entry = await context.MeterDimensionTest.AsNoTracking().Where(d => d.Id == Id).Select(d => new MeterDTO
        {
            Id = d.Id,
            RequiredMeter = d.RequiredMeter,
            OptionalMeter = d.OptionalMeter,
        }).FirstOrDefaultAsync();

        return entry;
    }

    private static async Task<DistanceDTO?> GetDistance(EntityContext context, int Id)
    {
        var entry = await context.DistanceDimensionTest.AsNoTracking().Where(d => d.Id == Id).Select(d => new DistanceDTO
        {
            Id = d.Id,
            RequiredDistance = d.RequiredDistance.GetDimension(),
            OptionalDistance = d.OptionalDistance.GetDimension()
        }).FirstOrDefaultAsync();

        return entry;
    }
}
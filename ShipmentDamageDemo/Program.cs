using JasperFx.CodeGeneration;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Oakton;
using ShipmentDamageDemo.Domain;
using ShipmentDamageDemo.Projections;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyOaktonExtensions();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.Services.AddMartenStore<>()


builder.Services.AddMarten(opts =>
    {
        opts.GeneratedCodeMode = TypeLoadMode.Static;
        opts.Connection("Host=localhost;Database=shipmentDamages;Username=root;Password=root");
        opts.Projections.Add<DamageReportsPendingReviewProjection>(ProjectionLifecycle.Async);
        opts.Projections.Add<CarrierDamageReportProjection>(ProjectionLifecycle.Async);

        //opts.RegisterDocumentType<CarrierDamageReport>();
        //opts.RegisterDocumentType<DamageReportsPendingReview>();
        opts.Schema.For<Carrier>().Identity(x => x.CarrierId);

        opts.Projections.LiveStreamAggregation<Carrier>();
        opts.Projections.LiveStreamAggregation<DamagedShipment>();
    }).AddAsyncDaemon(DaemonMode.HotCold)
    .UseLightweightSessions();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//app.Run();

await app.RunOaktonCommands(args);
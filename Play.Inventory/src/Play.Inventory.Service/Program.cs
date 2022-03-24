using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Play.Common.MongoDB;
using Play.Common.Settings;
using Play.Inventory.Service;
using Play.Inventory.Service.Consumers;
using Play.Inventory.Service.Dtos.Entities;

const string allowedOriginString = "AllowedOrigin";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ServiceSettings>(
    builder.Configuration.GetSection(nameof(ServiceSettings)));

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection(nameof(MongoDbSettings)));

builder.Services
    .AddMongo()
    .AddMongoRepository<InventoryItem>("inventoryitems")
    .AddMongoRepository<CatalogItem>("catalogitems");

builder.Services.AddMassTransit(serviceCollectionBusConfigurator =>
{
    serviceCollectionBusConfigurator.UsingRabbitMq((context, configuration) =>
    {
        IConfiguration service = context.GetService<IConfiguration>();
        RabbitMQSettings rabbitMqSettings = service.GetSection("RabbitMQSettings").Get<RabbitMQSettings>();
        ServiceSettings serviceSettings = service.GetSection("ServiceSettings").Get<ServiceSettings>();
        configuration.Host(rabbitMqSettings.Host);
        configuration.ConfigureEndpoints<IRabbitMqReceiveEndpointConfigurator>(context,
            new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
    });
    
    serviceCollectionBusConfigurator.AddConsumer<CatalogItemCreatedConsumer>();
    serviceCollectionBusConfigurator.AddConsumer<CatalogItemDeletedConsumer>();
    serviceCollectionBusConfigurator.AddConsumer<CatalogItemUpdatedConsumer>();
});
builder.Services.AddMassTransitHostedService();

builder.Services.AddCatalogClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.UseCors(policyBuilder =>
    {
        var origin = builder.Configuration[allowedOriginString];
        policyBuilder.WithOrigins(origin)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

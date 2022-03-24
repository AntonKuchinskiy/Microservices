using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Play.Catalog.Service.Entities;
using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Common.Settings;

const string allowedOriginString = "AllowedOrigin";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ServiceSettings>(
    builder.Configuration.GetSection(nameof(ServiceSettings)));

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection(nameof(MongoDbSettings)));

builder.Services.AddMongo()
    .AddMongoRepository<Item>("items")
    .AddRabbitMq();

// builder.Services.AddMassTransit(x =>
// {
//     // var qwe = AppDomain.CurrentDomain.GetAssemblies();
//     // var assembly = Assembly.GetEntryAssembly();
//     // serviceCollectionBusConfigurator.AddConsumer(qwe);
// });

//IConfiguration 
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

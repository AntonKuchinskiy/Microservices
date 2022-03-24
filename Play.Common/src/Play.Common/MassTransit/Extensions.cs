using System;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services)
        {
            services.AddMassTransit(serviceCollectionBusConfigurator =>
            {
                serviceCollectionBusConfigurator.UsingRabbitMq((context, configuration) =>
                {
                    var configurationService = context.GetService<IConfiguration>();
                    var rabbitMQSettings = configurationService.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                    var serviceSettings = configurationService.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                    
                    configuration.Host(rabbitMQSettings.Host);
                    configuration.ConfigureEndpoints(context,
                        new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                        
                    configuration.UseMessageRetry(configurator =>
                    {
                        configurator.Interval(3, TimeSpan.FromSeconds(5));
                    });
                });
                
                services.AddMassTransitHostedService();
            });

            return services;
        }
    }
}
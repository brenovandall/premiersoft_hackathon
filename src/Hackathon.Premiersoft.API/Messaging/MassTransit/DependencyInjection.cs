using MassTransit;
using System.Reflection;

namespace Hackathon.Premiersoft.API.Messaging.MassTransit
{
    internal static class DependencyInjection
    {
        public static IServiceCollection AddMessageBrokers(
            this IServiceCollection services,
            IConfiguration configuration,
            Assembly? assembly)
        {
            services.AddMassTransit(config =>
            {
                config.SetKebabCaseEndpointNameFormatter();

                if (assembly != null)
                {
                    config.AddConsumers(assembly);
                }

                config.AddDelayedMessageScheduler();

                config.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(new Uri(configuration["MessageBroker:Host"]!), host =>
                    {
                        host.Username(configuration["MessageBroker:UserName"]!);
                        host.Password(configuration["MessageBroker:Password"]!);
                    });

                    configurator.UseDelayedMessageScheduler();
                });
            });

            return services;
        }
    }
}

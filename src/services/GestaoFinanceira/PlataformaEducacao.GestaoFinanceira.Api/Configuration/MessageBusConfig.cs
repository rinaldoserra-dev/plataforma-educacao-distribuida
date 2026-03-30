using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlataformaEducacao.Core.Utils;
using PlataformaEducacao.MessageBus;
using PlataformaEducacao.GestaoFinanceira.Api.Services;

namespace PlataformaEducacao.GestaoFinanceira.Api.Configuration
{
    public static class MessageBusConfig
    {
        public static IServiceCollection AddMessageBusConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"));

            return services;
        }
    }
}

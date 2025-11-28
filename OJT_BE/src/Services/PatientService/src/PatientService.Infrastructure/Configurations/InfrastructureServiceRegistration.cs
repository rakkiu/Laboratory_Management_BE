using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PatientService.Application.Interfaces;
using PatientService.Infrastructure.Services;
using System;

namespace PatientService.Infrastructure.Configurations
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpClient<IIamEmailClient, IamEmailClient>(client =>
            {
                var baseUrl = configuration["IAMService:BaseUrl"];
                if (string.IsNullOrWhiteSpace(baseUrl))
                    throw new InvalidOperationException("IAMService:BaseUrl is missing in configuration.");

                client.BaseAddress = new Uri(baseUrl);
            });

            return services;
        }
    }
}

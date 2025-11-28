using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PatientService.Application;
using PatientService.Application.Behaviors;
using PatientService.Application.Interfaces;
using PatientService.Domain.Interfaces;
using PatientService.Domain.Interfaces.TestOrderService;          // Interface
using PatientService.Infrastructure.Data;
using PatientService.Infrastructure.Repositories;
using PatientService.Infrastructure.Repositories.TestOrderService; // Repository Implementation
using PatientService.Infrastructure.Security;
using PatientService.Infrastructure.Services;
using PatientService.Presentation.Filters;

namespace PatientService.Presentation.Extentions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
        {
            // Encryption key configuration
            var encryptionKey = config["EnvironmentVariables:DATA_ENCRYPTION_KEY"];
            EncryptionHelper.ConfigureKey(encryptionKey!);

            // DbContext abstraction
            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<PatientDbContext>());

            // Repositories
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<ITestOrderRepository, TestOrderRepository>();
            services.AddScoped<ITestResultRepository, TestResultRepository>();
            services.AddScoped<ITestResultDetailRepository, TestResultDetailRepository>();
            services.AddScoped<IPatientMedicalRecordRepository, PatientMedicalRecordRepository>();
            services.AddScoped<IFlaggingSetRepository, FlaggingSetRepository>();
            services.AddScoped<IEncryptionService, EncryptionService>();

            // Reporting
            services.AddScoped<IPdfService, PdfService>();
            services.AddScoped<IExcelService, ExcelService>();

            // MediatR config
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<ApplicationAssemblyMarker>();
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                cfg.AddOpenBehavior(typeof(AuditBehavior<,>));
                cfg.AddOpenBehavior(typeof(TestOrderAuditBehavior<,>));
            });

            // FluentValidation
            services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);

            return services;
        }
    }
}

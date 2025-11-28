using FluentValidation;
using FluentValidation.AspNetCore;
using IAMService.Application;
using IAMService.Application.Models.Email;
using IAMService.Application.Services;
using IAMService.Domain.Interfaces;
using IAMService.Infrastructure.Identity; // Added this line to resolve JwtService
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Security;
using System;

namespace IAMService.Presentation.Extentions
{
    /// <summary>
    /// Dependency Injection configuration for IAM Service
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the application services.
        /// </summary>
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
        {
            // 🔹 Database - Skip in Testing environment
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment != "Testing")
            {
                services.AddDbContext<IAMDbContext>(opt =>
                    opt.UseNpgsql(config.GetConnectionString("DefaultConnection")));
            }
            var encryptionKey = config["EnvironmentVariables:DATA_ENCRYPTION_KEY"];
            EncryptionHelper.ConfigureKey(encryptionKey!);
            services.Configure<EmailSettings>(config.GetSection("EmailSettings"));

            services.AddTransient<IEmailService, SmtpEmailService>();

            // 🔹 Dependency Injection
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IJwtTokenRepository, JwtTokenRepository>();
            services.AddScoped<IJwtService, IAMService.Infrastructure.Identity.JwtService>(); // Cleaned, no need to fully qualify
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRolePrivilegeRepository, RolePrivilegeRepository>();
            services.AddScoped<IPrivilegeRepository, PrivilegeRepository>();
            // 🔹 MediatR, AutoMapper, FluentValidation
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ApplicationAssemblyMarker>());
            services.AddAutoMapper(typeof(ApplicationAssemblyMarker).Assembly);
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);

            // 🔹 JWT Authentication
            var key = System.Text.Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"]!);

            services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = config["JwtSettings:Issuer"],
                        ValidAudience = config["JwtSettings:Audience"],
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.FromSeconds(30),

                        // ✅ Important: map Role claim properly for [Authorize(Roles = "...")]
                        RoleClaimType = "RoleCode",
                        NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier
                    };
                });

            // 🔹 Authorization
            services.AddAuthorization();

            return services;
        }
    }
}

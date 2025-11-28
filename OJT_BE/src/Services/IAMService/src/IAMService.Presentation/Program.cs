using IAMService.Infrastructure.BackgroundJobs;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Configures the JWT settings from appsettings.json and registers them for dependency injection.
/// </summary>
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

/// <summary>
/// Registers all application, infrastructure, and domain-level services.
/// Includes MediatR handlers, repositories, DbContext, etc.
/// </summary>
builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddGrpc();

// ---------------------------------------------
// JWT, Token, and Background Cleanup Services
// ---------------------------------------------
builder.Services.AddScoped<IJwtService, IAMService.Infrastructure.Identity.JwtService>();
builder.Services.AddScoped<IJwtTokenRepository, JwtTokenRepository>();

// Only add background service if not in testing environment
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddHostedService<RefreshTokenCleanupService>(); // auto cleanup job
}

/// <summary>
/// Configures Cross-Origin Resource Sharing (CORS) to allow requests from any origin.
/// </summary>
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

/// <summary>
/// Adds authentication and authorization services for role-based access control.
/// </summary>
builder.Services.AddAuthorization();

/// <summary>
/// Adds controller support to handle API endpoints.
/// </summary>
builder.Services.AddControllers();

/// <summary>
/// Enables API documentation and interactive Swagger UI.
/// </summary>
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "IAMService API",
        Version = "v1",
        Description = "Identity and Access Management Service API"
    });

    // --- JWT Token Security Configuration ---
    var jwtSecurityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Description = "Enter 'Bearer {your token}' to authorize requests.",
        Reference = new Microsoft.OpenApi.Models.OpenApiReference
        {
            Id = "Bearer",
            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition("Bearer", jwtSecurityScheme);
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

/// <summary>
/// Disables automatic 400 responses from invalid model states
/// to allow custom handling via ApiResponse objects.
/// </summary>
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();
app.MapGrpcService<IAMService.Presentation.Services.UserServiceGrpc>();

/// <summary>
/// Automatically applies pending EF Core migrations and seeds initial data
/// during application startup (skip in testing environment).
/// </summary>
if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<IAMDbContext>();
        db.Database.Migrate();
        await DbSeeder.SeedAsync(db);
    }
}

/// <summary>
/// Configures the HTTP request pipeline, including Swagger, middleware,
/// authentication, and routing.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/// <summary>
/// Global exception handling middleware to intercept unhandled exceptions
/// and return standardized error responses.
/// </summary>
app.UseMiddleware<ExceptionMiddleware>();

/// <summary>
/// Enables the defined CORS policy (AllowAll).
/// </summary>
app.UseCors("AllowAll");

/// <summary>
/// Adds authentication middleware to validate JWT tokens.
/// </summary>
app.UseAuthentication();

/// <summary>
/// Adds authorization middleware to enforce access policies.
/// </summary>
app.UseAuthorization();

/// <summary>
/// Maps controller endpoints for all API routes.
/// </summary>
app.MapControllers();

/// <summary>
/// Starts the web application.
/// </summary>
app.Run();

// Make the implicit Program class public so integration tests can access it
public partial class Program { }

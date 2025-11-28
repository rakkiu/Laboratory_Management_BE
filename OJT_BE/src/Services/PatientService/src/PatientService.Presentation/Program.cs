using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PatientService.Application.Interfaces;
using PatientService.Domain.Interfaces;
using PatientService.Infrastructure.Configurations;
using PatientService.Infrastructure.Data;
using PatientService.Infrastructure.Grpc;
using PatientService.Infrastructure.Repositories;
using PatientService.Infrastructure.Seed;
using PatientService.Presentation.Extentions;
using PatientService.Presentation.Filters;
using Shared.GrpcContracts;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// =========================
//     DATABASE
// =========================
builder.Services.AddDbContext<PatientDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// =========================
//   CONTROLLERS + FILTERS
// =========================
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationExceptionFilter>();
});

// =========================
//         SWAGGER
// =========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Patient Service API",
        Version = "v1"
    });

    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT Bearer token only.",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new Microsoft.OpenApi.Models.OpenApiReference
        {
            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

// =========================
//   APPLICATION SERVICES
// =========================
builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
// =========================
//         MEDIATR
// =========================
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(PatientService.Application.UseCases.CommentsUC.Add.AddCommentCommand).Assembly
    )
);

// =========================
//      REPOSITORIES
// =========================
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IIamUserClient, IamUserClient>();
builder.Services.AddGrpcClient<UserService.UserServiceClient>(o =>
{
    o.Address = new Uri("https://localhost:5001");
});

// =========================
//   🔥 ADD IHttpContextAccessor
// =========================
builder.Services.AddHttpContextAccessor();
// 💡 Quan trọng: handler mới dùng được _httpContextAccessor

// =========================
//    AUTHENTICATION - JWT
// =========================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = configuration.GetSection("JwtSettings");
    var issuer = jwtSettings["Issuer"];
    var audience = jwtSettings["Audience"];
    var secretKey = jwtSettings["SecretKey"];

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

// =========================
//       AUTHORIZATION
// =========================
builder.Services.AddAuthorization();

// =========================
//       BUILD APP
// =========================
var app = builder.Build();

// =========================
//       AUTO MIGRATION
// =========================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<PatientDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    dbContext.Database.Migrate();
    await DbSeeder.SeedData(dbContext, logger);
}

// =========================
//       MIDDLEWARE
// =========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 🔥 MUST be in this order:
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

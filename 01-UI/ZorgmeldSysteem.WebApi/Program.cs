using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ZorgmeldSysteem.Application.Interfaces.IServices;
using ZorgmeldSysteem.Application.Services;
using ZorgmeldSysteem.Domain.Configuration;
using ZorgmeldSysteem.Infrastructure.Configuration;
using ZorgmeldSysteem.Persistence.Services;

var builder = WebApplication.CreateBuilder(args);

// ===================================
// FLY.IO CONFIGURATIE
// ===================================
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

// ===================================
// CONTROLLERS & SWAGGER
// ===================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Zorgmeld Systeem API",
        Version = "v1",
        Description = "API voor het ZorgmeldSysteem - Ticket Management"
    });
});

// ===================================
// DATABASE
// ===================================
builder.Services.AddDatabase(builder.Configuration);

// ===================================
// JWT AUTHENTICATION
// ===================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

// ⭐ Lees ALLE JWT settings uit environment variables (Fly.io) of User Secrets (lokaal)
// Prioriteit: 1) Environment Variable (Fly.io), 2) Configuration (User Secrets), 3) Error
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? jwtSettings.Get<JwtSettings>()?.SecretKey
    ?? throw new InvalidOperationException("JWT SecretKey not configured");

var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
    ?? jwtSettings.Get<JwtSettings>()?.Issuer
    ?? throw new InvalidOperationException("JWT Issuer not configured");

var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
    ?? jwtSettings.Get<JwtSettings>()?.Audience
    ?? throw new InvalidOperationException("JWT Audience not configured");

var expirationMinutes = int.TryParse(
    Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES"),
    out var expMinutes)
    ? expMinutes
    : (jwtSettings.Get<JwtSettings>()?.ExpirationMinutes ?? 480);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ===================================
// APPLICATION SERVICES
// ===================================
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IMechanicService, MechanicService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IObjectService, ObjectService>();

// ===================================
// CORS
// ===================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ===================================
// BUILD & CONFIGURE PIPELINE
// ===================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Zorgmeld API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazor");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
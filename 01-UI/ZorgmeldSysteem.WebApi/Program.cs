using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ZorgmeldSysteem.Application.Interfaces.IServices; // ✅ Add this!
using ZorgmeldSysteem.Application.Services;
using ZorgmeldSysteem.Domain.Configuration;
using ZorgmeldSysteem.Infrastructure.Configuration;
using ZorgmeldSysteem.Persistence.Services;

var builder = WebApplication.CreateBuilder(args);

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

var secretKey = jwtSettings.Get<JwtSettings>()?.SecretKey
    ?? throw new InvalidOperationException("JWT SecretKey not configured");

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
        ValidIssuer = jwtSettings.Get<JwtSettings>()?.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Get<JwtSettings>()?.Audience,
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
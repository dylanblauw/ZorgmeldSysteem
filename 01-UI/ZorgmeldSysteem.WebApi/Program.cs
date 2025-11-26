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
if (!builder.Environment.IsDevelopment())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(int.Parse(port));
    });
}

// ===================================
// CONTROLLERS & SWAGGER
// ===================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Fixility API",
        Version = "v1.0",
        Description = "API voor het Fixility Ticket Management Systeem"
    });
});

// ===================================
// DATABASE
// ===================================
builder.Services.AddDatabase(builder.Configuration);




// ===================================
// JWT AUTHENTICATION \\morgen aanpassen zorgen dat hij uit secrets.json haalt en niet hier echt fysiek neerzet
// ===================================
builder.Configuration.AddUserSecrets<Program>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? jwtSettings.SecretKey;
var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? jwtSettings.Issuer;
var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? jwtSettings.Audience;


Console.WriteLine($"[DEBUG] SecretKey length: {secretKey?.Length ?? 0}");
Console.WriteLine($"[DEBUG] Issuer: {issuer}");
Console.WriteLine($"[DEBUG] Audience: {audience}");



if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
    throw new InvalidOperationException("JWT configuratie ontbreekt!");
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

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
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Fixility API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazor");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using ZorgmeldSysteem.Application.Interfaces.IServices;
//using ZorgmeldSysteem.Application.Services;
//using ZorgmeldSysteem.Domain.Configuration;
//using ZorgmeldSysteem.Infrastructure.Configuration;
//using ZorgmeldSysteem.Persistence.Services;

//var builder = WebApplication.CreateBuilder(args);

//// ===================================
//// FLY.IO CONFIGURATIE
//// ===================================
//if (!builder.Environment.IsDevelopment())
//{
//    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
//    builder.WebHost.ConfigureKestrel(options =>
//    {
//        options.ListenAnyIP(int.Parse(port));
//    });
//}

//// ===================================
//// CONTROLLERS & SWAGGER
//// ===================================
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
//    {
//        Title = "Fixility API",
//        Version = "v1.0",
//        Description = "API voor het Fixility Ticket Management Systeem"
//    });
//});

//// ===================================
//// DATABASE
//// ===================================
//builder.Services.AddDatabase(builder.Configuration);




//// ===================================
//// JWT AUTHENTICATION \\morgen aanpassen zorgen dat hij uit secrets.json haalt en niet hier echt fysiek neerzet
//// ===================================
//builder.Configuration.AddUserSecrets<Program>();

//var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

//var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? jwtSettings.SecretKey;
//var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? jwtSettings.Issuer;
//var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? jwtSettings.Audience;


//Console.WriteLine($"[DEBUG] SecretKey length: {secretKey?.Length ?? 0}");
//Console.WriteLine($"[DEBUG] Issuer: {issuer}");
//Console.WriteLine($"[DEBUG] Audience: {audience}");



//if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
//    throw new InvalidOperationException("JWT configuratie ontbreekt!");
//builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
//        ValidateIssuer = true,
//        ValidIssuer = issuer,
//        ValidateAudience = true,
//        ValidAudience = audience,
//        ValidateLifetime = true,
//        ClockSkew = TimeSpan.Zero
//    };
//});

//builder.Services.AddAuthorization();

//// ===================================
//// APPLICATION SERVICES
//// ===================================
//builder.Services.AddScoped<JwtService>();
//builder.Services.AddScoped<AuthService>();
//builder.Services.AddScoped<IMechanicService, MechanicService>();
//builder.Services.AddScoped<ITicketService, TicketService>();
//builder.Services.AddScoped<ICompanyService, CompanyService>();
//builder.Services.AddScoped<IObjectService, ObjectService>();

//// ===================================
//// CORS
//// ===================================
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowBlazor", policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyHeader()
//              .AllowAnyMethod();
//    });
//});

//// ===================================
//// BUILD & CONFIGURE PIPELINE
//// ===================================
//var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(options =>
//    {
//        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Fixility API v1");
//        options.RoutePrefix = "swagger";
//    });
//}

//app.UseHttpsRedirection();
//app.UseCors("AllowBlazor");
//app.UseAuthentication();
//app.UseAuthorization();
//app.MapControllers();

//app.Run();

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
// JWT AUTHENTICATION
// ===================================
// User Secrets toevoegen (alleen in development)
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// JWT configuratie ophalen en valideren
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings sectie ontbreekt in configuratie!");

var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? jwtSettings.SecretKey;
var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? jwtSettings.Issuer;
var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? jwtSettings.Audience;

// Debug logging (alleen in development)
if (builder.Environment.IsDevelopment())
{
    Console.WriteLine($"[DEBUG] SecretKey length: {secretKey?.Length ?? 0}");
    Console.WriteLine($"[DEBUG] Issuer: {issuer}");
    Console.WriteLine($"[DEBUG] Audience: {audience}");
}

// Validatie
if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
    throw new InvalidOperationException("JWT configuratie ontbreekt! Controleer appsettings.json en environment variables.");

// JWT Settings registreren
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// JWT Authentication configureren
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
        if (builder.Environment.IsDevelopment())
        {
            // Development: alle origins toestaan voor makkelijker testen
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            // Production: alleen specifieke origins toestaan
            // Pas deze URLs aan naar jouw daadwerkelijke Blazor app URLs
            policy.WithOrigins(
                      "https://jouw-blazor-app.fly.dev",
                      "https://jouw-custom-domain.nl"
                  )
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Belangrijk voor authenticated requests
        }
    });
});

// ===================================
// BUILD & CONFIGURE PIPELINE
// ===================================
var app = builder.Build();

// Swagger (development en optioneel production)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Fixility API v1");
        options.RoutePrefix = "swagger";
    });
}

// HTTPS Redirection alleen in development
// Op Fly.io gebeurt SSL terminatie op de load balancer
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// CORS moet voor Authentication/Authorization
app.UseCors("AllowBlazor");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

// Startup bericht
Console.WriteLine($"[INFO] Fixility API gestart in {app.Environment.EnvironmentName} mode");
Console.WriteLine($"[INFO] Listening on port: {Environment.GetEnvironmentVariable("PORT") ?? "default"}");

app.Run();

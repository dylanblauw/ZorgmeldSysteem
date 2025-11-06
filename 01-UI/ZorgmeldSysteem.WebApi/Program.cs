using ZorgmeldSysteem.Application.Interfaces.IServices;
using ZorgmeldSysteem.Application.Services;
using ZorgmeldSysteem.Infrastructure.Configuration;
using ZorgmeldSysteem.Persistence.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ NIEUW: CORS toevoegen voor Blazor
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7052",  // Blazor HTTPS
            "http://localhost:5030",   // Blazor HTTP
            "https://localhost:44386"  // IIS Express SSL
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IMechanicService, MechanicService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IObjectService, ObjectService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ NIEUW: CORS toepassen (VOOR UseAuthorization!)
app.UseCors("AllowBlazor");

app.UseAuthorization();

app.MapControllers();

app.Run();
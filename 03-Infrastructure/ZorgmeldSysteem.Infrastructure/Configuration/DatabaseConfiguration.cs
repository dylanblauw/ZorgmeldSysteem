using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZorgmeldSysteem.Persistence.Context;

namespace ZorgmeldSysteem.Infrastructure.Configuration
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Probeer eerst environment variable (Fly.io), anders appsettings
            var connectionString =
                Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
                ?? configuration.GetConnectionString("ZorgmeldDatabase");

            services.AddDbContext<ZorgmeldContext>(options =>
                options.UseSqlServer(connectionString,
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null)));

            return services;
        }
    }
}
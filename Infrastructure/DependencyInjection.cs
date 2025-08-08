using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        var connectionString = "Host=db;Database=fullsearch;Username=postgres;Password=postgres"
            ?? throw new InvalidOperationException("Connection string not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString
                ?? throw new InvalidOperationException("Connection string not found.")));

        // services.AddDbContext<ApplicationDbContext>(options =>
        //     options.UseNpgsql(Environment.GetEnvironmentVariable("ConnectionString")
        //         ?? throw new InvalidOperationException("Connection string not found.")));

        return services;
    }

    public static IServiceCollection AddOpenSearch(this IServiceCollection services)
    {
        return services;
    }
    
}

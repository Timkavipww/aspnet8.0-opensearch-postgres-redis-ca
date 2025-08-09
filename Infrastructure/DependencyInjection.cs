using Domain.Constants.Indicies;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenSearch.Client;
using OpenSearch.Net;

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
        var connectionPool = new SingleNodeConnectionPool(new Uri("http://opensearch:9200"));

        var settings = new ConnectionSettings(connectionPool)
            .DefaultIndex(OpenSearchIndicies.BOOK_INDEX)
            .PrettyJson()
            .EnableHttpCompression()
            .DisableDirectStreaming()
            .DefaultFieldNameInferrer(f => f.ToLower())
            .DefaultMappingFor<Book>(m => m.IndexName(OpenSearchIndicies.BOOK_INDEX)) 
            .DefaultMappingFor<Author>(m => m.IndexName(OpenSearchIndicies.AUTHOR_INDEX)) 
            .BasicAuthentication("admin", "SuperSecurePassword123");

        var client = new OpenSearchClient(settings);

        services.AddSingleton<IOpenSearchClient>(client);

        return services;
    }
    
}

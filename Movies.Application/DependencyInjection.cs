using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Database;
using Movies.Application.Repositories;
using Movies.Application.Services;

namespace Movies.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IMovieRepository, MovieRepository>();
        services.AddSingleton<IMovieService, MovieService>(); //singleton because there is no shared state. in movie service.

        // services.AddValidatorsFromAssembly(typeof(ApplicationServiceCollectionExtensions).Assembly); // cannot customise ServiceLifetime.
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton); // singleton is safe, because dependence on other singleton and no state.

        services.AddSingleton<IRatingRepository, RatingRepository>();
        services.AddSingleton<IRatingService, RatingService>();
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection service, string connectionString)
    {
        // Type-Based registration. this is not working it gives error. Because NpgsqlDbConnectionFactory construcutor is not able to resolve connectionstring passed as string.
        // if you want to use this instead of getting connection string request IConfiguration
        // service.AddSingleton<IDbConnectionFactory, NpgsqlDbConnectionFactory>();
        
        // factory-based registration
        service.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlDbConnectionFactory(connectionString));
        // this is singleton masking a transient. because CreateConnectionAsync() method returns 'new' instance everytime.

        service.AddSingleton<DbInitializer>();
        
        return service;
    }
}

// Application should no nothing about the system that consumes it.(It can be any thing mvc,razor pages, api)
// so we will use the microsoft package dependecyinjection.abstration 
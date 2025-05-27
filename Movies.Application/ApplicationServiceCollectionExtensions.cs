using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Repositories;

namespace Movies.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IMovieRepository,MovieRepository>(); 
        // adding as a singleton. because we want only one instance of it throughout my application since it has by 'db in memory'. which i don't want to reset
        return services;
    }
}

// Application should no nothing about the system that consumes it.(It can be any thing mvc,razor pages, api)
// so we will use the microsoft package dependecyinjection.abstration 
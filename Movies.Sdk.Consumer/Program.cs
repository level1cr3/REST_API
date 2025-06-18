using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Movies.Contracts.Requests.V1;
using Movies.Sdk;
using Refit;

// var moviesApi = RestService.For<IMoviesApi>("https://localhost:5001");

// var movie = moviesApi.GetMovieAsync("the-clean-greek-2024");

// Console.WriteLine(JsonSerializer.Serialize(movie));


// installed dependency injection package and refit http client factory
var services = new ServiceCollection();

services.AddRefitClient<IMoviesApi>().ConfigureHttpClient(options =>
{
    options.BaseAddress = new Uri("https://localhost:5001");
}); 

// we can configure httpclient and any of the handler here as well.
// this will take care of handling httpclient with httpclient factory.



var provider = services.BuildServiceProvider();

var moviesApi = provider.GetRequiredService<IMoviesApi>();

var getAllMoviesRequest = new GetAllMoviesRequest
{
    SortBy = null,
    Title = null,
    YearOfRelease = null,
    Page = 1,
    PageSize = 3
};

var movies = await moviesApi.GetMoviesAsync(getAllMoviesRequest);

foreach (var movieResponse in movies.Items)
{
    Console.WriteLine(JsonSerializer.Serialize(movieResponse));
}

/*
 return object is returned only on 2XX status codes response. any bad path will throw apiException and this exception needs to parsed to get appropriate erros.
 */
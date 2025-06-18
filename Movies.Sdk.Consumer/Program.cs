using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Movies.Contracts.Requests.V1;
using Movies.Sdk;
using Movies.Sdk.Consumer;
using Refit;

// var moviesApi = RestService.For<IMoviesApi>("https://localhost:5001");


// installed dependency injection package and refit http client factory
var services = new ServiceCollection();

services.AddHttpClient();
services.AddSingleton<AuthTokenProvider>();

services.AddRefitClient<IMoviesApi>(serviceProvider => new RefitSettings
    {
        AuthorizationHeaderValueGetter = async (HttpRequestMessage message, CancellationToken token) => await serviceProvider.GetRequiredService<AuthTokenProvider>().GetTokenAsync(),
    }).ConfigureHttpClient(options => { options.BaseAddress = new Uri("https://localhost:5001"); });

// we can configure httpclient and any of the handler here as well.
// this will take care of handling httpclient with httpclient factory.


var provider = services.BuildServiceProvider();

var moviesApi = provider.GetRequiredService<IMoviesApi>();



// var movie = moviesApi.GetMovieAsync("the-clean-greek-2024");

// Console.WriteLine(JsonSerializer.Serialize(movie));

    var newMovie = await moviesApi.CreateMovieAsync(new CreateMovieRequest
    {
        Title = "Spiderman 2",
        YearOfRelease = 2002,
        Genres =
        [
            "Action"
        ]

    });


Console.WriteLine(JsonSerializer.Serialize(newMovie));

var updateMovie= await moviesApi.UpdateMovieAsync(newMovie.Id, new UpdateMovieRequest
{
    Title = "Spiderman 2",
    YearOfRelease = 2002,
    Genres =
    [
        "Action",
        "Adventure"
    ]

});

Console.WriteLine(JsonSerializer.Serialize(updateMovie));

await moviesApi.DeleteMovieAsync(newMovie.Id);


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
 
 if we provide this sdk consider putting all service related code in good extension method. so user can just call 
 that extension method and start working or consuming the api.
 */
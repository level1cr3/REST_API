using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Movies.Contracts.Requests.V1;
using Movies.Sdk;
using Refit;

// var moviesApi = RestService.For<IMoviesApi>("https://localhost:5001");


// installed dependency injection package and refit http client factory
var services = new ServiceCollection();

services.AddRefitClient<IMoviesApi>(options => new RefitSettings
    {
        AuthorizationHeaderValueGetter = delegate
        {
            return Task.FromResult(
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyOGEwNjdlOS0yODgyLTRlMjMtYWE1OS00YmYyM2ZlOTM3ZjgiLCJzdWIiOiJuaWNrQG5pY2tjaGFwc2FzLmNvbSIsImVtYWlsIjoibmlja0BuaWNrY2hhcHNhcy5jb20iLCJ1c2VyaWQiOiJkODU2NmRlMy1iMWE2LTRhOWItYjg0Mi04ZTM4ODdhODJlNDEiLCJhZG1pbiI6dHJ1ZSwidHJ1c3RlZF9tZW1iZXIiOnRydWUsIm5iZiI6MTc1MDIzOTc2NiwiZXhwIjoxNzUwMjY4NTY2LCJpYXQiOjE3NTAyMzk3NjYsImlzcyI6Imh0dHBzOi8veW9nZXNoLmRvdG5ldC5jb20iLCJhdWQiOiJodHRwczovL3lvZ2VzaC5yZWFjdC5jb20ifQ.z7rytgEDwVQ2IsjR_wNMlN9uvN4D5KpEZ-TCxRRlCJQ");
        }
    })
    .ConfigureHttpClient(options => { options.BaseAddress = new Uri("https://localhost:5001"); });

// we can configure httpclient and any of the handler here as well.
// this will take care of handling httpclient with httpclient factory.


var provider = services.BuildServiceProvider();

var moviesApi = provider.GetRequiredService<IMoviesApi>();



var movie = moviesApi.GetMovieAsync("the-clean-greek-2024");

Console.WriteLine(JsonSerializer.Serialize(movie));


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
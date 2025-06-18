using System.Text.Json;
using Movies.Contracts.Requests.V1;
using Movies.Sdk;
using Refit;

var moviesApi = RestService.For<IMoviesApi>("https://localhost:5001");

// var movie = moviesApi.GetMovieAsync("the-clean-greek-2024");

// Console.WriteLine(JsonSerializer.Serialize(movie));

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
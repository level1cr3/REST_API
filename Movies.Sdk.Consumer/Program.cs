using System.Text.Json;
using System.Text.Json.Serialization;
using Movies.Sdk;
using Refit;

var moviesApi = RestService.For<IMoviesApi>("https://localhost:5001");

var movie = moviesApi.GetMovieAsync("the-clean-greek-2024");

Console.WriteLine(JsonSerializer.Serialize(movie));


/*
 return object is returned only on 2XX status codes response. any bad path will throw apiException and this exception needs to parsed to get appropriate erros.
 */
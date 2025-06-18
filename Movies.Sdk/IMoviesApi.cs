using Movies.Contracts.Requests.V1;
using Movies.Contracts.Responses.V1;
using Refit;

namespace Movies.Sdk;


[Headers("Authorization: Bearer")]
public interface IMoviesApi
{
    [Get(ApiEndpoints.V1.Movies.Get)]
    Task<MovieResponse> GetMovieAsync(string idOrSlug);

    [Get(ApiEndpoints.V1.Movies.GetAll)]
    Task<MoviesResponse> GetMoviesAsync(GetAllMoviesRequest request);
}
using Movies.Contracts.Responses.V1;
using Refit;

namespace Movies.Sdk;


public interface IMoviesApi
{
    [Get(ApiEndpoints.V1.Movies.Get)]
    Task<MovieResponse> GetMovieAsync(string idOrSlug);
}
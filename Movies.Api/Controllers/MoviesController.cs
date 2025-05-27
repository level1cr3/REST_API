using Microsoft.AspNetCore.Mvc;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
[Route("api")]
public class MoviesController(IMovieRepository movieRepository) : ControllerBase
{
    [HttpPost("movies")]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
    {
        var movie = new Movie
        {
            Id = Guid.CreateVersion7(),
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
        
        var isCreated = await movieRepository.CreateAsync(movie);

        var responseObj = new MovieResponse
        {
            Id = movie.Id,
            Title = movie.Title,
            YearOfRelease = movie.YearOfRelease,
            Genres = movie.Genres
        };
        
        return isCreated ? Created($"/api/movies/{responseObj.Id}",responseObj) : BadRequest();
        
        // for created method. it returns url in the response location header like this. and the object.
        // Location : /api/movies/01971155-a6a1-734f-88e7-f1718c848b49
    }

}
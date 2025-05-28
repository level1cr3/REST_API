using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Api.Routes;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController(IMovieRepository movieRepository) : ControllerBase
{
    [HttpPost(ApiEndpoints.Movies.Create)] // this way i don't have to use route attribute explicitly.
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
    {
        var movie = request.MapToMovie();
        var isCreated = await movieRepository.CreateAsync(movie);
        var responseObj = movie.MapToMovieResponse();
        
        return isCreated ? Created($"/{ApiEndpoints.Movies.Create}/{responseObj.Id}",responseObj) : BadRequest();
        
        // for created method. it returns url in the response location header like this. and the object.
        // Location : /api/movies/01971155-a6a1-734f-88e7-f1718c848b49
    }
    
    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var movie = await movieRepository.GetByIdAsync(id);

        if (movie is null)
        {
            return NotFound();
        }

        var movieResponse = movie.MapToMovieResponse();
        return Ok(movieResponse);
    }

    // it important to not expose domain object outside. and only use contracts. for request and response. Because contract are supposed to be fixed.

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var movies = await movieRepository.GetAllAsync();
        var moviesResponse = movies.MapToMoviesResponse();
        return Ok(moviesResponse);
    }


}
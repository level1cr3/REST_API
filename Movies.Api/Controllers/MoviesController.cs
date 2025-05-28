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
    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
    {
        var movie = request.MapToMovie();
        
        var isCreated = await movieRepository.CreateAsync(movie);

        var responseObj = movie.MapToMovieResponse();
        
        return isCreated ? Created($"/{ApiEndpoints.Movies.Create}/{responseObj.Id}",responseObj) : BadRequest();
        
        // for created method. it returns url in the response location header like this. and the object.
        // Location : /api/movies/01971155-a6a1-734f-88e7-f1718c848b49
    }

}
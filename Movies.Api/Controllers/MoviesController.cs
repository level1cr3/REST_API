using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Api.Routes;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController(IMovieService movieService) : ControllerBase
{
    [HttpPost(ApiEndpoints.Movies.Create)] // this way i don't have to use route attribute explicitly.
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
    {
        var movie = request.MapToMovie();
        var isCreated = await movieService.CreateAsync(movie);
        var responseObj = movie.MapToMovieResponse();

        // return isCreated ? Created($"/{ApiEndpoints.Movies.Create}/{responseObj.Id}",responseObj) : BadRequest();

        // for created method. it returns url in the response location header like this. and the object.
        // Location : /api/movies/01971155-a6a1-734f-88e7-f1718c848b49

        return isCreated ? CreatedAtAction(nameof(Get), new { idOrSlug = responseObj.Id }, responseObj) : BadRequest();
        // use CreatedAtAction() it is better then Created in terms of giving value to location header.
        // provides us with full contextual location of the item.
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug)
    {
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await movieService.GetByIdAsync(id)
            : await movieService.GetBySlugAsync(idOrSlug);

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
        var movies = await movieService.GetAllAsync();
        var moviesResponse = movies.MapToMoviesResponse();
        return Ok(moviesResponse);
    }


    // update has both route parameter and request body
    // route parameter has id of resource they want to update.
    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request)
    {
        var movie = request.MapToMovie(id);
        var updatedMovie = await movieService.UpdateAsync(movie);

        if (updatedMovie is null)
        {
            return NotFound();
        }

        var response = updatedMovie.MapToMovieResponse();
        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var isDeleted = await movieService.DeleteByIdAsync(id);
        return isDeleted ? Ok() : NotFound();
    }

    /*
     Why partial updates are bad ?
        Reason it is complex to build the PATCH request. and process the PATCH request.
        It is way simpler for the client to use GET request to get the Item they want to update. make changes in that and use PUT to update it.

        HTTPPATCH has fallen out of favour.

        This is what general PATCH request would look like
        [
            {
                "op" : "replace",
                "path" : "/name",
                "value" : "Green house"
            },
            {
                "op" : "add",
                "path" : "/bedroom",
                "value" : {
                    "name" : "bedroom",
                    "color" : "blue"
                }

            }
        ]

        It is way complicated to create in client and also complicated to process in server side. Hence we won't use PATCH
    */
}
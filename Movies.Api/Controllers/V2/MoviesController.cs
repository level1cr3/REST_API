using System.ComponentModel;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Extensions;
using Movies.Api.Mapping;
using Movies.Api.Routes;
using Movies.Application.Services;

namespace Movies.Api.Controllers.V2;

// [ApiVersion(3.0)] we can set it on controller level as well.
[ApiController]
public class MoviesController(IMovieService movieService) : ControllerBase
{
    [ApiVersion(2.0, Deprecated = true)]
    [HttpGet(ApiEndpoints.V2.Movies.Get)]
    public async Task<IActionResult> GetV2([FromRoute] string idOrSlug,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();

        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await movieService.GetByIdAsync(id, userId, cancellationToken)
            : await movieService.GetBySlugAsync(idOrSlug, userId, cancellationToken);

        if (movie is null)
        {
            return NotFound();
        }

        var movieResponse = movie.MapToMovieResponse();
        return Ok(movieResponse);
    }

    [ApiVersion(3.0)]
    [HttpGet(ApiEndpoints.V2.Movies.Get)]
    public async Task<IActionResult> GetV3([FromRoute] string idOrSlug,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();

        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await movieService.GetByIdAsync(id, userId, cancellationToken)
            : await movieService.GetBySlugAsync(idOrSlug, userId, cancellationToken);

        if (movie is null)
        {
            return NotFound();
        }

        var movieResponse = movie.MapToMovieResponse();
        return Ok(movieResponse);
    }
}

// you cannot mark api endpoint as deprecated if that api-version is the default version in program.cs


// there are 4 standard ways to version your api.
// 1. url segment : that we used in basic versioning.
// 2. Query string : the one where we can pass the version in query string. https://myapi.com/api/books?api-version = 3.0
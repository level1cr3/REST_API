using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Constants;
using Movies.Api.Extensions;
using Movies.Api.Filters;
using Movies.Api.Mapping;
using Movies.Api.Routes;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;
using Movies.Contracts.Responses.V1;

namespace Movies.Api.Controllers.V1;

[ApiVersion(1.0)]
[ApiController]
public class MoviesController(IMovieService movieService, IOutputCacheStore outputCacheStore) : ControllerBase
{
    [Authorize(AuthConstants.TrustedOrAdminUserPolicyName)]
    // [ServiceFilter(typeof(ApiAuthKeyFilter))] //older
    // [ServiceFilter<ApiAuthKeyFilter>] // new from C# 11+
    
    [HttpPost(ApiEndpoints.V1.Movies.Create)] // this way i don't have to use route attribute explicitly.
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken cancellationToken)
    {
        var movie = request.MapToMovie();
        var isCreated = await movieService.CreateAsync(movie, cancellationToken);
        var responseObj = movie.MapToMovieResponse();

        // return isCreated ? Created($"/{ApiEndpoints.Movies.Create}/{responseObj.Id}",responseObj) : BadRequest();

        // for created method. it returns url in the response location header like this. and the object.
        // Location : /api/movies/01971155-a6a1-734f-88e7-f1718c848b49

        await outputCacheStore.EvictByTagAsync("movies", cancellationToken);
        return isCreated ? CreatedAtAction(nameof(Get), new { idOrSlug = responseObj.Id }, responseObj) : BadRequest();
        // use CreatedAtAction() it is better then Created in terms of giving value to location header.
        // provides us with full contextual location of the item.
    }

    
    [HttpGet(ApiEndpoints.V1.Movies.Get)] // method HTTPGET needs to be added for response cache.
    [ProducesResponseType(typeof(MovieResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    // [ResponseCache(Duration = 30, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)] // vary by accept, accept-encoding because we don't want to give json data to someone who request data in xml.
    [OutputCache(PolicyName = "MovieCache")]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, [FromServices] LinkGenerator linkGenerator,
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

        var movieObj = new { id = movie.Id };
        
        movieResponse.Links =
        [
            new Link
            {
                Href = linkGenerator.GetPathByAction(HttpContext, nameof(Get), values: new { idOrSlug = movie.Id }),
                Rel = "self",
                Type = "GET"
            },
            new Link
            {
                Href = linkGenerator.GetPathByAction(HttpContext, nameof(Update), values: movieObj),
                Rel = "self",
                Type = "PUT"
            }
        ];

        
        return Ok(movieResponse);
    }
    
    
    // it important to not expose domain object outside. and only use contracts. for request and response. Because contract are supposed to be fixed.
    [HttpGet(ApiEndpoints.V1.Movies.GetAll)]
    [ProducesResponseType(typeof(MoviesResponse),StatusCodes.Status200OK)]
    // [ResponseCache(Duration = 30, VaryByQueryKeys = ["title", "YearOfRelease", "SortBy", "Page", "PageSize"], VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
    [OutputCache(PolicyName = "MovieCache")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest request,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var options = request.MapToMoviesOptions().WithUser(userId);
        var movies = await movieService.GetAllAsync(options, cancellationToken);
        var totalCount = await movieService.GetCountAsync(options.Title, options.Year, cancellationToken);
        var moviesResponse = movies.MapToMoviesResponse(options.Page, options.PageSize, totalCount);
        return Ok(moviesResponse);
    }


    // update has both route parameter and request body
    // route parameter has id of resource they want to update.
    [Authorize(AuthConstants.TrustedOrAdminUserPolicyName)]
    [HttpPut(ApiEndpoints.V1.Movies.Update)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var movie = request.MapToMovie(id);
        var updatedMovie = await movieService.UpdateAsync(movie, userId, cancellationToken);

        if (updatedMovie is null)
        {
            return NotFound();
        }

        var response = updatedMovie.MapToMovieResponse();
        await outputCacheStore.EvictByTagAsync("movies", cancellationToken);
        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.V1.Movies.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        // test multi auth.
        var userId = HttpContext.GetUserId();
        
        var isDeleted = await movieService.DeleteByIdAsync(id, cancellationToken);
        await outputCacheStore.EvictByTagAsync("movies", cancellationToken);
        return isDeleted ? Ok() : NotFound();
    }

}
    /*
     
     response caching:
        > Don't cache CREATE, UPDATE, DELETE methods.
        > Only cache GET by id Or slug and GET-ALL
   
     > You have to be careful with response caching. we always need to specify the 'vary constraint'
     like tell it should vary on which types/ query parameters and so on.
     
     > response cache is completely based on client and not on the server. if they don't want to use cached response they can say that.
     > we don't control the cache here it is just instructions.
        
         
     
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

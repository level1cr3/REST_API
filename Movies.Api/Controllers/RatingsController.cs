using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Extensions;
using Movies.Api.Mapping;
using Movies.Api.Routes;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController]
public class RatingsController(IRatingService ratingService) : ControllerBase
{
    [Authorize]
    [HttpPut(ApiEndpoints.Movies.Rate)]
    public async Task<IActionResult> RateMovie([FromRoute] Guid id, [FromBody] RateMovieRequest request,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var result = await ratingService.RateMovieAsync(userId!.Value, id, request.Rating, cancellationToken);
        // used null-forgiving symbol because. we have Authorize attribute that means userId must be there.

        return result ? Ok() : NotFound();
    }
    
    [Authorize]
    [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
    public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var result = await ratingService.DeleteRatingAsync(userId!.Value,id,cancellationToken);
        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
    public async Task<IActionResult> GetUserRatings(CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var movieRatings = await ratingService.GetAllRatingsForUserAsync(userId!.Value, cancellationToken);
        var movieRatingResponse = movieRatings.MapToMovieRatingResponse();
        
        return Ok(movieRatingResponse);
    }


    
}
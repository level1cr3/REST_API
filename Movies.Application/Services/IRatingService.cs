using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IRatingService
{
    Task<bool> RateMovieAsync(Guid userId, Guid movieId, int rating, CancellationToken cancellationToken = default);

    Task<bool> DeleteRatingAsync(Guid userId, Guid movieId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<MovieRating>> GetAllRatingsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
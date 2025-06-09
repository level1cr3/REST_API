namespace Movies.Application.Services;

public interface IRatingService
{
    Task<bool> RateMovieAsync(Guid userId, Guid movieId, int rating, CancellationToken cancellationToken = default);
}
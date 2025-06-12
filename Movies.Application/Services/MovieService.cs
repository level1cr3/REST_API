using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public sealed class MovieService(
    IMovieRepository movieRepository,
    IRatingRepository ratingRepository,
    IValidator<Movie> movieValidator,
    IValidator<GetAllMoviesOptions> getAllMoviesOptionsValidator) : IMovieService
{
    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userid = null, CancellationToken cancellationToken = default)
    {
        return await movieRepository.GetByIdAsync(id, userid, cancellationToken);
    }

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userid = null,
        CancellationToken cancellationToken = default)
    {
        return await movieRepository.GetBySlugAsync(slug, userid, cancellationToken);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options,
        CancellationToken cancellationToken = default)
    {
        await getAllMoviesOptionsValidator.ValidateAndThrowAsync(options, cancellationToken: cancellationToken);
        options.Title = options.Title?.Trim().ToLower();
        return await movieRepository.GetAllAsync(options, cancellationToken);
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        // var result = await movieValidator.ValidateAsync(movie); // explicitly handle validation failure. decide what to show to user.
        await movieValidator.ValidateAndThrowAsync(movie,
            cancellationToken); // if not valid throws error. which we could catch in global exception middleware and return that .
        return await movieRepository.CreateAsync(movie, cancellationToken);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, Guid? userid = null,
        CancellationToken cancellationToken = default)
    {
        await movieValidator.ValidateAndThrowAsync(movie, cancellationToken: cancellationToken);
        // adding business logic
        var movieExists = await movieRepository.ExistsByIdAsync(movie.Id, cancellationToken);

        if (!movieExists)
        {
            return null;
        }

        await movieRepository.UpdateAsync(movie, cancellationToken);

        if (userid.HasValue)
        {
            // if userId is there it is good to get user ratings from db as well. because that will be single source of truth.
            
            var ratingFromDb = await ratingRepository.GetRatingAsync(movie.Id, userid.Value, cancellationToken);
            movie.Rating = ratingFromDb.Rating;
            movie.UserRating = ratingFromDb.UserRating;
            
            return movie;
        }
        
        movie.Rating = await ratingRepository.GetRatingAsync(movie.Id, cancellationToken);
        return movie;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await movieRepository.DeleteByIdAsync(id, cancellationToken);
    }
}
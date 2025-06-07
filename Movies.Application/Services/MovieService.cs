using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public sealed class MovieService(IMovieRepository movieRepository, IValidator<Movie> movieValidator) : IMovieService
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

    public async Task<IEnumerable<Movie>> GetAllAsync(Guid? userid = null,
        CancellationToken cancellationToken = default)
    {
        return await movieRepository.GetAllAsync(userid, cancellationToken);
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

        await movieRepository.UpdateAsync(movie, userid, cancellationToken);
        return movie;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await movieRepository.DeleteByIdAsync(id, cancellationToken);
    }
}
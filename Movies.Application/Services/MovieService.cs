using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

internal sealed class MovieService(IMovieRepository movieRepository) : IMovieService
{
    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        return await movieRepository.GetByIdAsync(id);
    }

    public async Task<Movie?> GetBySlugAsync(string slug)
    {
        return await movieRepository.GetBySlugAsync(slug);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        return await movieRepository.GetAllAsync();
    }

    public async Task<bool> CreateAsync(Movie movie)
    {
        return await movieRepository.CreateAsync(movie);
    }

    public async Task<Movie?> UpdateAsync(Movie movie)
    {
        // adding business logic
        
        var movieExists = await movieRepository.ExistsByIdAsync(movie.Id);

        if (!movieExists)
        {
            return null;
        }

        await movieRepository.UpdateAsync(movie);
        return movie;
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        return await movieRepository.DeleteByIdAsync(id);
    }
    
}
using Movies.Application.Models;

namespace Movies.Application.Repositories;

internal interface IMovieRepository
{
    Task<Movie?> GetByIdAsync(Guid id);
    
    Task<Movie?> GetBySlugAsync(string slug);

    Task<IEnumerable<Movie>> GetAllAsync();

    Task<bool> CreateAsync(Movie movie);

    Task<bool> UpdateAsync(Movie movie);

    Task<bool> DeleteByIdAsync(Guid id);

    Task<bool> ExistsByIdAsync(Guid id);
}
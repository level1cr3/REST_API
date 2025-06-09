using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IMovieRepository
{
    Task<Movie?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default);

    Task<Movie?> GetBySlugAsync(string slug, Guid? userId = null, CancellationToken cancellationToken = default);

    Task<IEnumerable<Movie>> GetAllAsync(Guid? userid = null, CancellationToken cancellationToken = default);

    Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
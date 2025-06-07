using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IMovieService
{
    Task<Movie?> GetByIdAsync(Guid id, Guid? userid = null, CancellationToken cancellationToken = default);
    
    Task<Movie?> GetBySlugAsync(string slug, Guid? userid = null, CancellationToken cancellationToken = default);

    Task<IEnumerable<Movie>> GetAllAsync(Guid? userid = null, CancellationToken cancellationToken = default);

    Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default); // default value here is CancellationToken.None

    Task<Movie?> UpdateAsync(Movie movie, Guid? userid = null, CancellationToken cancellationToken = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

// If we had DTO's we would take them as the input and return output as DTO's
// and we would have mapper in application layer for converting to dto.
// so repository will take enitty model as input and return entity model as output right.


// Keep your repository pure — it should return entities, not DTOs.
// Do mapping in the service layer, where business logic and transformation belong.

// It is not necessary to have DTO's every time if model is simple you could use all the way from repo to services level.
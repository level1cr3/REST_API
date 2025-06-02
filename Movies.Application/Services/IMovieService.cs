using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IMovieService
{
    Task<Movie?> GetByIdAsync(Guid id);
    
    Task<Movie?> GetBySlugAsync(string slug);

    Task<IEnumerable<Movie>> GetAllAsync();

    Task<bool> CreateAsync(Movie movie);

    Task<Movie?> UpdateAsync(Movie movie);

    Task<bool> DeleteByIdAsync(Guid id);
}

// If we had DTO's we would take them as the input and return output as DTO's
// and we would have mapper in application layer for converting to dto.
// so repository will take enitty model as input and return entity model as output right.


// Keep your repository pure — it should return entities, not DTOs.
// Do mapping in the service layer, where business logic and transformation belong.

// It is not necessary to have DTO's every time if model is simple you could use all the way from repo to services level.
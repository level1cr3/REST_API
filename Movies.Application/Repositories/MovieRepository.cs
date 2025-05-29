using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly List<Movie> _movies = []; // for now this will act as in memory db.

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        var movie = _movies.FirstOrDefault(m => m.Id == id);
        return Task.FromResult(movie);
    }

    public Task<Movie?> GetBySlugAsync(string slug)
    {
        var movie = _movies.FirstOrDefault(m => m.Slug == slug);
        return Task.FromResult(movie);
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        return Task.FromResult(_movies.AsEnumerable());
    }

    public Task<bool> CreateAsync(Movie movie)
    {
        var ogCount = _movies.Count;
        _movies.Add(movie);
        var newCount = _movies.Count;
        return Task.FromResult(newCount > ogCount);
    }

    public Task<bool> UpdateAsync(Movie movie)
    {
        var index = _movies.FindIndex(m => m.Id == movie.Id);

        if (index == -1)
        {
            return Task.FromResult(false);
        }

        _movies[index] = movie;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteByIdAsync(Guid id)
    {
        var removeCount = _movies.RemoveAll(m => m.Id == id);
        return Task.FromResult(removeCount > 0);
    }
}
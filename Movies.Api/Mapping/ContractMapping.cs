using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Mapping;

public static class ContractMapping
{
    // mapping should be simple that you shouldn't rely on third party library.
    // ##Important## : Don't put business logic in here it should be simply mapping.

    public static Movie MapToMovie(this CreateMovieRequest request)
    {
        return new Movie
        {
            Id = Guid.CreateVersion7(),
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
    }

    public static MovieResponse MapToMovieResponse(this Movie movie)
    {
        return new MovieResponse
        {
            Id = movie.Id,
            Slug = movie.Slug,
            Title = movie.Title,
            YearOfRelease = movie.YearOfRelease,
            Genres = movie.Genres,
            Rating = movie.Rating,
            UserRating = movie.UserRating
        };
    }

    public static MoviesResponse MapToMoviesResponse(this IEnumerable<Movie> movies, int page, int pageSize, int totalCount)
    {
        var moviesResponseList = movies.Select(MapToMovieResponse);

        return new MoviesResponse
        {
            Items = moviesResponseList,
            Page =  page,
            PageSize =  pageSize,
            Total =  totalCount
        };
    }

    public static Movie MapToMovie(this UpdateMovieRequest request, Guid id)
    {
        return new Movie
        {
            Id = id,
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
    }

    public static IEnumerable<MovieRatingResponse> MapToMovieRatingResponse(this IEnumerable<MovieRating> movieRatings)
    {
        return movieRatings.Select(m => new MovieRatingResponse
        {
            MovieId = m.MovieId,
            Slug = m.Slug,
            Rating = m.Rating
        });
    }

    public static GetAllMoviesOptions MapToMoviesOptions(this GetAllMoviesRequest request)
    {
        return new GetAllMoviesOptions
        {
            Title = request.Title,
            Year = request.YearOfRelease,
            SortField = request.SortBy?.Trim('+','-'),
            
            /* 
             nested ternary is bad.
            SortOrder = request.SortBy is null
                ? SortOrder.Unsorted
                : request.SortBy.StartsWith('-')
                    ? SortOrder.Descending
                    : SortOrder.Ascending
            */
            
            SortOrder = request.SortBy switch
            {
                null => SortOrder.Unsorted,
                var sort when sort.StartsWith('-') => SortOrder.Descending,
                _ => SortOrder.Ascending
            },
            
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public static GetAllMoviesOptions WithUser(this GetAllMoviesOptions options, Guid? userId)
    {
        options.UserId = userId;
        return options;
    }

}


/*
 
var sort            // 1. Declares a variable 'sort'
when               // 2. Introduces a condition
sort.StartsWith('-') // 3. The condition to check
=>                 // 4. Arrow syntax for the result
SortOrder.Descending // 5. The value to return if condition is true
 
*/
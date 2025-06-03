using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;

namespace Movies.Application.Validators;

public sealed class MovieValidator : AbstractValidator<Movie>
{
    // private readonly IMovieService _movieService;
    // ##### Important #########
    // should not inject service in validators. because we will use these validations inside the services only
    // so this could lead to circular dependency because service dependence on validator and validator depends on service
    // So we will use repository instead.

    private readonly IMovieRepository _movieRepository;
    
    public MovieValidator(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
        
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Genres).NotEmpty();
        
        RuleFor(x => x.Title).NotEmpty();
        
        RuleFor(x => x.YearOfRelease).LessThanOrEqualTo(DateTime.UtcNow.Year); // no future movies

        // MustAsync() for custom validation.
        RuleFor(x => x.Slug).MustAsync(ValidateSlug).WithMessage("This movie already exists in the system");

    }

    private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken cancellationToken = default)
    {
        var existingMovie = await _movieRepository.GetBySlugAsync(slug);
        return existingMovie is null || existingMovie.Id == movie.Id; 
        // either there should not be any movie. for create senario.
        // if there is a movie it's id should match with one we are updating.
        
        // another way of writing the above code.
        // if (existingMovie is not null)
        // {
        //     return existingMovie.Id == movie.Id;  
        // }
        //
        // return existingMovie is null;
    }
}

// fluentvalidation.
// notempty() :  for guid means not empty.guid
// and for list it means not empty list.  It is contextual.


// slug validations : They are custom validation.
// for create : should not exist (not be there)
// for update : It can exist as long as the movie Id is matching the movie we are updating. 


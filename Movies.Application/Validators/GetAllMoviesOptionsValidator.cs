using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validators;

public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesOptions>
{
    private static readonly string[] acceptedSortFields = ["title","yearofrelease"];
    
    public GetAllMoviesOptionsValidator()
    {
        RuleFor(x => x.Year).LessThanOrEqualTo(DateTime.UtcNow.Year);

        RuleFor(x => x.SortField)
            .Must(x => x is null || acceptedSortFields.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage("You can only sort by 'title' or 'yearofrelease'");
    }
}
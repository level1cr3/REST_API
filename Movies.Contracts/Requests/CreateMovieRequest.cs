namespace Movies.Contracts.Requests;

public record CreateMovieRequest
{
    public required string Title { get; init; }
    
    public required int YearOfRelease { get; init; }
    
    public required IEnumerable<string> Genres { get; init; } = [];
}


// General guideline:
//
// Internal objects: required keyword for compile-time safety
// External boundaries (APIs, forms): [Required] attribute for runtime validation
// Both: When you want compile-time safety AND runtime validation (common in web APIs)
//
// The required keyword is newer and provides better developer experience, while [Required] integrates with existing validation infrastructure.
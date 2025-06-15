namespace Movies.Contracts.Responses.V1;

public record MovieRatingResponse
{
    public required Guid MovieId { get; init; }
    
    public required string Slug { get; init; }
    
    public required int Rating { get; init; }
}

// we could also create MoviesRatingResponse. similar to how we did for get movie and get all movies
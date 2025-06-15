namespace Movies.Contracts.Responses.V1;

public record MovieResponse : HalResponse
{
    public required Guid Id { get; init; }

    public required string Slug { get; init; }
    
    public required string Title { get; init; }

    public float? Rating { get; init; }
    
    public int? UserRating { get; init; }
    
    public required int YearOfRelease { get; init; }

    public required IEnumerable<string> Genres { get; init; } = [];
}

// responses will include 2 ratings. The general rating for the movie and also the suer specific ratings.
// they will also be nullable.
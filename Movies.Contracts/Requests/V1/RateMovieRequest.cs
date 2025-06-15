namespace Movies.Contracts.Requests.V1;

public record RateMovieRequest()
{
    public required int Rating { get; init; } 
}
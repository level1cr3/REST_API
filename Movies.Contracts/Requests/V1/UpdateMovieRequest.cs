namespace Movies.Contracts.Requests.V1;

public record UpdateMovieRequest
{
    public required string Title { get; init; }

    public required int YearOfRelease { get; init; }

    public required IEnumerable<string> Genres { get; init; } = [];
}

// Why we are using two contracts one for create and one for update even though they both have the same properties ?
// we didn't use same contract for both request is because as we are developing the api
// we might need to add property one object which is not supposed to be in others.

// WHY not delete contract ?
// The reason for not having delete contract is that delete only depends on the identifier of the record.
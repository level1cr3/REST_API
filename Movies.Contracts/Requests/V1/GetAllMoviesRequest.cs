﻿namespace Movies.Contracts.Requests.V1;

public record GetAllMoviesRequest : PagedRequest
{
    // filtering
    public required string? Title { get; init; } // using trim,lowercase and LIKE in sql.
    
    public required int? YearOfRelease { get; init; }
    
}

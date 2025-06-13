namespace Movies.Contracts.Requests;

public record PagedRequest
{
    public required int Page { get; init; } = 1;

    public required int PageSize { get; init; } = 10;
    
    public required string? SortBy { get; init; } 
}


/*
We have to be smart about which fields we allow users to sort.
Because It will affect the performance of the database. Be careful with sorting.

this is how sorting will work we will have SortBy query parameter. here we will pass the column name or property name
by which we want to sort.

for sort order

sortby?=title  // sorts ascending
sortby?=-title // sorts descending order

*/
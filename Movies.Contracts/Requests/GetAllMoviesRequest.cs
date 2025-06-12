namespace Movies.Contracts.Requests;

public record GetAllMoviesRequest
{
    // filtering
    public required string? Title { get; init; } // using trim,lowercase and LIKE in sql.
    
    public required int? YearOfRelease { get; init; }
    
    // sorting.
    // sortby?=title   sorts ascending
    // sortby?=-title  sorts descending order
    
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
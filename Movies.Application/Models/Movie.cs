namespace Movies.Application.Models;

public class Movie
{
    public required Guid Id { get; init; }

    public required string Title { get; set; } // not making init because title can change.
    
    public required int YearOfRelease { get; set; } // not making init because year of release can change.

    public required List<string> Genres { get; init; } = []; // making it init. since it is 'List' it will still allow me to add remove the genres. Because list is mutable data structure.
}
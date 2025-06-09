using System.Text.RegularExpressions;

namespace Movies.Application.Models;

public partial class Movie
{
    public required Guid Id { get; init; }

    public required string Title { get; set; } // not making init because title can change.

    public required int YearOfRelease { get; set; } // not making init because year of release can change.

    public float? Rating { get; set; }
    
    public int? UserRating { get; set; }
    
    public required List<string> Genres { get; init; } =
        []; // making it init. since it is 'List' it will still allow me to add remove the genres. Because list is mutable data structure.

    public string Slug => GetSlug();

    private string GetSlug()
    {
        // var input = Regex.Replace(Title, @"[^0-9a-z\s]", string.Empty).Trim().ToLower();
        var input = SlugRegex().Replace(Title, string.Empty).Trim().ToLower();
        
        // replace white spaces with hyphen.
        // input = Regex.Replace(input, @"\s+", "-");
        input = SpaceRegex().Replace(input, "-");

        return $"{input}-{YearOfRelease}";
    }

    // ^ is 'not' in regex or negation. \s represents white space.
    [GeneratedRegex(@"[^a-zA-Z0-9\s]", RegexOptions.NonBacktracking)]
    private static partial Regex SlugRegex();
    
    [GeneratedRegex(@"\s+", RegexOptions.NonBacktracking)]
    private static partial Regex SpaceRegex();
}

/*
why do we need slug based retrieval ?

We use slug-based retrieval to create clean, readable, and SEO-friendly URLs. Instead of using IDs like /posts/123, we use slugs like /posts/how-to-code-in-csharp. This improves:

    ✅ User experience (easy to read and remember)

    ✅ SEO (keywords in URL help search engines)

    ✅ Sharing (more meaningful and descriptive links)

It’s especially useful for blogs, products, or articles where titles matter.

Keep in mind it alternate identifier. But main identifier is still Id.

================================================================================>

// Regex vs GaneratedRegex

############### Regex

✅ Pros:
Simple and familiar

Works in all .NET versions

Easy to read and modify

❌ Cons:
Regex is parsed and compiled at runtime, which may add overhead if used repeatedly

No compile-time validation of regex string

Less performant in hot paths (like loops or high-frequency APIs)

################ GeneratedRegex

✅ Pros:
Compile-time code generation → faster at runtime (no runtime parsing)

More efficient for frequently used or performance-critical regex

Safer: validates pattern at compile time

Reduces runtime allocations

❌ Cons:
Requires .NET 7 or later

Slightly more complex syntax

Regex methods must be defined in a partial class

*/

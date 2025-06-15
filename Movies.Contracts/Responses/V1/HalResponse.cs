using System.Text.Json.Serialization;

namespace Movies.Contracts.Responses.V1;

public abstract record HalResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] // won't include in response if links are null.
    public List<Link> Links { get; set; } 

}

public record Link
{
    public required string Href { get; init; }
    
    public required string Rel { get; init; }
    
    public required string Type { get; init; }
}


// HAL : stands for hypermedia api language.
namespace Movies.Api.Extensions;

public static class IdentityExtensions
{
    public static Guid? GetUserId(this HttpContext context)
    {
        // context.User.Claims.SingleOrDefault(claim =>  claim is { Type: "userid"} ); // use it only when it makes your code clean 
        
        var userId= context.User.Claims.SingleOrDefault(claim =>  claim.Type == "userid" );
        // using single or default because there shouldn't be more then one claim for it
        
        return Guid.TryParse(userId?.Value, out var parsedId) ? parsedId : null;
    }
    
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Movies.Api.Constants;

namespace Movies.Api.Filters;

public class ApiAuthKeyFilter(IConfiguration configuration) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var apiKeyFromRequest))
        {
            // context.Result = new UnauthorizedResult(); maybe in production we might want to go with this.
            context.Result = new UnauthorizedObjectResult("API key is missing");
            return;
        }

        var apiKey = configuration["ApiKey"];

        if (apiKey != apiKeyFromRequest)
        {
            context.Result = new UnauthorizedObjectResult("Invalid API key");
        }
        
    }
}

// For async implement this IAsyncAuthorizationFilter

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Movies.Api.Constants;

namespace Movies.Api.Auth;

public class AdminAuthHandler : AuthorizationHandler<AdminAuthRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminAuthRequirement requirement)
    {
        if (context.User.HasClaim(AuthConstants.AdminUserClaimName,"true"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        // var httpContext = context.Resource as HttpContext;
        // if(httpContext is null)
        // {
        //     return Task.CompletedTask;
        // }
        
        // Get HttpContext (make sure this is an endpoint auth scenario)
        if (context.Resource is not HttpContext httpContext)
        {
            return Task.CompletedTask;
        }
        // if not httpContext return it otherwise declare variable httpContext with the value.

        if (!httpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var apiKeyFromRequest))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        if (requirement.ApiKey != apiKeyFromRequest)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        // optionally add claim to apiKey. maybe from db. we should store apikey in db/config

       var identity = (ClaimsIdentity)httpContext.User.Identity!;
       identity.AddClaim(new Claim("userid", Guid.Parse("b639e15e-9d60-4a19-a18d-6f9c9c6c3e73").ToString()));
       context.Succeed(requirement);
       return Task.CompletedTask;


    }
}
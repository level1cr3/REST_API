using System.Net.Mime;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Movies.Api.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ValidationException validationEx)
        {
           var validationErrors = validationEx.Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());
            
            var validationProblemDetails = new ValidationProblemDetails(validationErrors)
            {
                Title = "Validation Failure",
                Status = StatusCodes.Status400BadRequest,
                // Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1" // link documentation for error if you have one.
            };

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            // httpContext.Response.ContentType = MediaTypeNames.Application.Json;
            httpContext.Response.ContentType = "application/problem+json"; // RFC 7807, standard
            await httpContext.Response.WriteAsJsonAsync(validationProblemDetails, cancellationToken);
            return true;
        }

        return false;
    }
}


// 1. Return true if you handled the exception and wrote the response.
// 2. Return false if you did not handle it (so another handler can try, or the default will run).
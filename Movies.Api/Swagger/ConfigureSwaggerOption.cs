using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Movies.Api.Swagger;

public class ConfigureSwaggerOption(IApiVersionDescriptionProvider provider, IHostEnvironment environment)
    : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var apiVersionDescription in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                apiVersionDescription.GroupName,
                new OpenApiInfo
                {
                    Title = environment.ApplicationName,
                    Version = apiVersionDescription.ApiVersion.ToString()
                });
        }

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please provide a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });


        // approach 1 :
        // var securityRequirement = new OpenApiSecurityRequirement();
        //
        // securityRequirement.Add(
        //     new OpenApiSecurityScheme
        //     {
        //         Reference = new OpenApiReference
        //         {
        //             Type = ReferenceType.SecurityScheme,
        //             Id = "Bearer"
        //         }
        //     }, []);
        //
        // options.AddSecurityRequirement(securityRequirement);

        // approach 2 : using collection initialization
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                []
            }
        });
        
    }
}

/*
 Yes, both approaches do exactly the same thing - they add a security requirement to the OpenAPI options with an empty array of scopes.


Here's how it works:
```csharp
// These are equivalent:

// Long way (like approach 1)
var dict = new Dictionary<string, int>();
dict.Add("key1", 1);
dict.Add("key2", 2);

// Short way with collection initializer
var dict = new Dictionary<string, int>
{
    { "key1", 1 },
    { "key2", 2 }
};
```


The difference is just in syntax style:

**Approach 1**: Creates the object first, then adds items to it
- Creates a new `OpenApiSecurityRequirement` object
- Uses the `.Add()` method to add a key-value pair
- Passes the populated object to `AddSecurityRequirement()`

**Approach 2**: Uses collection initializer syntax
- Creates and initializes the `OpenApiSecurityRequirement` object in one go
- Uses the `{{ }}` collection initializer syntax

The `{{ }}` syntax you're asking about is C#'s **collection initializer syntax**. It's a shorthand way to add items to a collection during object creation.

In your case:
- `OpenApiSecurityRequirement` implements `IDictionary<OpenApiSecurityScheme, IList<string>>`
- The outer `{ }` is the object initializer
- The inner `{ }` contains the dictionary key-value pairs
- Each `{ key, value }` pair gets added to the dictionary

So `{ securityScheme, [] }` means "add this security scheme as the key, with an empty array as the value".

Both approaches are valid - choose based on your preference for readability and consistency with your codebase.
 */
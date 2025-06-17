using System.Text;
using Asp.Versioning;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.Api.Constants;
using Movies.Api.Health;
using Movies.Api.Middleware;
using Movies.Api.Swagger;
using Movies.Application;
using Movies.Application.Database;
using Movies.Application.Repositories;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ??
                                                                           throw new InvalidOperationException(
                                                                               "Auth validation key not found"))),
        // key should not leak because that is what we are using to validate someone.
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ??
                      throw new InvalidOperationException("Auth valid issuer not found"),
        ValidAudience = builder.Configuration["Jwt:Audience"] ??
                        throw new InvalidOperationException("Auth valid audience not found"),

        // enable below settings or nothing will be validated.
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidateIssuer = true,
        ValidateAudience = true,
    }; // here we will decide how we want to validate the token.
});

builder.Services.AddAuthorization(option =>
{
    option.AddPolicy(AuthConstants.AdminUserPolicyName,
        policy => policy.RequireClaim(AuthConstants.AdminUserClaimName, "true"));

    option.AddPolicy(AuthConstants.TrustedOrAdminUserPolicyName, policy => policy.RequireAssertion(context =>
        context.User.HasClaim(AuthConstants.TrustedUserClaimName, "true") ||
        context.User.HasClaim(AuthConstants.AdminUserClaimName, "true")
    ));

    /* with pattern matching.
     
    option.AddPolicy(AuthConstants.TrustedOrAdminUserPolicyName, policy => policy.RequireAssertion(context =>
        context.User.HasClaim(claim => claim is { Type: AuthConstants.TrustedUserClaimName, Value: "true" }) ||
        context.User.HasClaim(claim => claim is { Type: AuthConstants.AdminUserClaimName, Value: "true" })
    ));
    
    */
    
});


// builder.Services.AddSingleton<IMovieRepository,MovieRepository>();
// Writing above line is not good idea why ?
// Because entire point of having application layer is to encapsulate it's logic in that layer only. By doing this it 
// implies that every consumer should know. How application concerns needs to be registered. which is not good way of 

var dbConnection = builder.Configuration.GetConnectionString("Database") ??
                   throw new InvalidOperationException("Connection string not found");
builder.Services.AddApplication();
builder.Services.AddDatabase(dbConnection);

//exception
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
//  Multiple handlers can be added and they're called by the middleware in the order they're added
// this is the middleware btw app.UseExceptionHandler(); 


// versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1.0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    // options.ApiVersionReader = new HeaderApiVersionReader("api-version"); having request header api versioning.
    
    
    options.ApiVersionReader = new MediaTypeApiVersionReader("api-version"); // send with accept header this is standard follow this

    // options.ApiVersionReader = new UrlSegmentApiVersionReader();
    // ai says to go with url segment. for versioning.
}).AddMvc().AddApiExplorer();// this will add mvc core that is needed by the package.


// add swagger
// builder.Services.AddEndpointsApiExplorer();

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOption>();
builder.Services.AddSwaggerGen(option =>
{
    option.OperationFilter<SwaggerDefaultValues>();
});


// health check
// custom health check
// builder.Services.AddHealthChecks().AddCheck<DatabaseHealthCheck>(DatabaseHealthCheck.Name);

// custom health check is not recommended. instead i'm gonna use the library for it. AspNetCore.HealthChecks.
builder.Services.AddHealthChecks().AddNpgSql(dbConnection);


// caching
builder.Services.AddResponseCaching();

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();

// adding underscore to the endpoint so client knows this endpoint is only for metadata not official endpoint. 
// every service should implement health check rest api, database and other service that you use.
app.MapHealthChecks("_health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();

    // this can also be outside of development based on your requirements
    app.UseSwagger();
    app.UseSwaggerUI(option =>
    {
        foreach (var apiVersionDescription in app.DescribeApiVersions())
        {
            option.SwaggerEndpoint($"/swagger/{apiVersionDescription.GroupName}/swagger.json", apiVersionDescription.GroupName);
        }
    });
}


app.UseAuthentication();
app.UseAuthorization();

// app.UseCors(); // don't put this below response caching.
app.UseResponseCaching();

app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

app.Run();


/*

GetService<T>()

Returns: The service instance if it is registered, or null if not.
Throws: Never throws if the service is missing (returns null instead).
Usage: Use when the dependency is optional.

GetRequiredService<T>()

Returns: The service instance if it is registered.
Throws: Throws an InvalidOperationException if the service is not registered.
Usage: Use when the dependency is required and your app cannot function without it.

*/
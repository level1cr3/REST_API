using Movies.Application;
using Movies.Application.Database;
using Movies.Application.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();


// builder.Services.AddSingleton<IMovieRepository,MovieRepository>();
// Writing above line is not good idea why ?
// Because entire point of having application layer is to encapsulate it's logic in that layer only. By doing this it 
// implies that every consumer should know. How application concerns needs to be registered. which is not good way of 

var dbConnection = builder.Configuration.GetConnectionString("Database") ?? throw new InvalidOperationException("Connection string not found");
builder.Services.AddApplication();
builder.Services.AddDatabase(dbConnection);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
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
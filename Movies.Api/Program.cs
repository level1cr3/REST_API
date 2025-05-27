using Movies.Application;
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

builder.Services.AddApplicationServices();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

internal sealed class MovieRepository(IDbConnectionFactory connectionFactory) : IMovieRepository
{
    private readonly List<Movie> _movies = []; // for now this will act as in memory db.

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        const string getMovieByIdSql = """
                                        select m.*, round(avg(r.rating), 1) as rating, myr.rating as userrating
                                        from movies m
                                        left join ratings r on r.movieid = m.id        
                                        left join ratings myr on myr.movieid = m.id and myr.userid = @userId
                                        where m.id = @id
                                        group by m.id, myr.rating;
                                       """;

        // we don't need limit. because we are querying by id primary key also we are using singleOrDefault

        var movie =
            await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition(getMovieByIdSql, new { id, userId },
                cancellationToken: cancellationToken));

        if (movie is null)
        {
            return null;
        }

        const string getGenresByMovieIdSql = """
                                              select name from genres 
                                              where movieid = @id; 
                                             """;

        var generes = await connection.QueryAsync<string>(new CommandDefinition(getGenresByMovieIdSql, new { id },
            cancellationToken: cancellationToken));

        // movie.Genres = generes; this won't work right now but if genres is big list. later on we could make this property from init to set 

        foreach (var genere in generes)
        {
            movie.Genres.Add(genere);
        }

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        const string getMovieBySlugSql = """
                                          select m.*, round(avg(r.rating), 1) as rating, myr.rating as userrating 
                                          from movies m        
                                          left join ratings r on r.movieid = m.id 
                                          left join ratings myr on myr.movieid = m.id and myr.userid = @userId
                                          where m.slug = @Slug
                                          group by m.id, myr.rating;
                                         """;

        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition(getMovieBySlugSql,
            new { Slug = slug }, cancellationToken: cancellationToken));

        if (movie is null)
        {
            return null;
        }

        const string getGenresByMovieIdSql = """
                                              select name from genres 
                                              where movieid = @id; 
                                             """;

        var genres =
            await connection.QueryAsync<string>(new CommandDefinition(getGenresByMovieIdSql, new { id = movie.Id },
                cancellationToken: cancellationToken));

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }


    public async Task<IEnumerable<Movie>> GetAllAsync(Guid? userid = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        const string getAllMovies = """
                                     select m.*, g.name AS genre, round(avg(r.rating),1) as rating, myr.rating as userrating 
                                     from movies m
                                     left join genres g on m.id = g.movieid 
                                     left join ratings r on r.movieid = m.id
                                     left join ratings myr on myr.movieid = m.id and myr.userid = @userId
                                     group by m.id, g.name, myr.rating
                                    """;

        var movieDict = new Dictionary<Guid, Movie>();

        await connection.QueryAsync<Movie, string, float, int, Movie>(
            new CommandDefinition(getAllMovies, cancellationToken: cancellationToken),
            (movie, genre, rating, userRating) =>
            {
                if (!movieDict.TryGetValue(movie.Id, out var movieEntry))
                {
                    // movie not in dictionary 
                    movieEntry = movie;
                    movieDict.Add(movieEntry.Id, movieEntry);
                }

                // this way multiple genres can be added to single movie. because that movie will come from dict
                movieEntry.Genres.Add(genre);
                movieEntry.Rating ??= rating;
                movieEntry.UserRating ??= userRating;
                return movieEntry;
            },
            splitOn: "genre,rating,userrating"
        );

        return movieDict.Values;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        try
        {
            const string createMovieSql = """
                                          insert into movies(id, slug, title, yearofrelease)
                                          values (@Id, @Slug, @Title, @YearOfRelease);
                                          """;

            // var createdMovieRowCount1 = await connection.ExecuteAsync(new CommandDefinition(createMovieSql, 
            //      new { Id = movie.Id, Slug = movie.Slug, Title = movie.Title, YearOfRelease = @movie.YearOfRelease }, transaction));

            // var createdMovieRowCount2 = await connection.ExecuteAsync(new CommandDefinition(createMovieSql, 
            //     new { movie.Id, movie.Slug, movie.Title, @movie.YearOfRelease }, transaction));


            var affectedRows =
                await connection.ExecuteAsync(new CommandDefinition(createMovieSql, movie, transaction,
                    cancellationToken: cancellationToken));
            // take values for parameter from object that i'm passing you that is the reason why params needs to be in pascal case similar to movie object.


            if (affectedRows > 0)
            {
                const string createGenreSql = """
                                              insert into genres(movieid, name) 
                                              values (@MovieId, @Name);
                                              """;

                foreach (var genre in movie.Genres)
                {
                    await connection.ExecuteAsync(
                        new CommandDefinition(createGenreSql, new { MovieId = movie.Id, Name = genre }, transaction,
                            cancellationToken: cancellationToken));
                }
            }

            transaction.Commit();
            return
                affectedRows >
                0; // don't need to have variable from createGenreSql, since transaction is same either all pass or all fail.
        }
        catch (Exception e)
        {
            transaction.Rollback();
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        try
        {
            // we will not individually look for what genres was updated,
            // we will go the lazy way by removing all existing genres and add new ones.

            const string removeGenresByMovieId = """ delete from genres where movieid = @movieId """;
            await connection.ExecuteAsync(new CommandDefinition(removeGenresByMovieId, new { movieId = movie.Id },
                transaction, cancellationToken: cancellationToken));

            const string addGenresForMovie = """
                                              insert into genres(movieid, name)
                                              VALUES (@movieId, @name); 
                                             """;
            // insert multiple rows.
            var genres = movie.Genres.Select(name => new { movieId = movie.Id, name });
            await connection.ExecuteAsync(new CommandDefinition(addGenresForMovie, genres, transaction,
                cancellationToken: cancellationToken));

            const string updateMovieSql = """
                                           update movies 
                                           set slug = @slug, title = @title, yearofrelease = @yearofrelease
                                           where id = @id;
                                          """;
            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(updateMovieSql,
                    new { slug = movie.Slug, title = movie.Title, yearofrelease = movie.YearOfRelease, id = @movie.Id },
                    transaction, cancellationToken: cancellationToken));

            transaction.Commit();
            return affectedRows > 0;
        }
        catch (Exception e)
        {
            transaction.Rollback();
            ;
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        try
        {
            const string deleteGenreSql = """
                                           delete from genres
                                           where movieid = @Id
                                          """;

            await connection.ExecuteAsync(new CommandDefinition(deleteGenreSql, new { Id = id }, transaction,
                cancellationToken: cancellationToken));


            const string deleteMovieSql = """
                                           delete from movies
                                           where id = @Id
                                          """;

            var affectedRows =
                await connection.ExecuteAsync(new CommandDefinition(deleteMovieSql, new { Id = id }, transaction,
                    cancellationToken: cancellationToken));


            transaction.Commit();
            return affectedRows > 0;
        }
        catch (Exception e)
        {
            transaction.Rollback();
            Console.WriteLine(e);
            throw;
        }
    }


    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        const string movieExistsSql = """select exists(select 1 from movies where id = @id);""";
        var result =
            await connection.ExecuteScalarAsync<bool>(new CommandDefinition(movieExistsSql, new { id },
                cancellationToken: cancellationToken));

        // `ExecuteScalarAsync<T>` | Get a **single value** (e.g. COUNT, ID, scalar) | using this because that is what it was made for.

        return result;
    }
}


/*
 1. Triple Double Quotes: """ ... """
   What are they called?
   These are called raw string literals (introduced in C# 11 / .NET 7 and later).

## When to use them?
   Use them when you:

>   Want to include multi-line text without any escape sequences.
>   Need to copy-paste text (like JSON, XML, SQL, or code) with quotes, backslashes, or special formatting, and you don’t want to escape anything.
>   Want to preserve the original formatting and indentation.

>   Example:
var json = """
   {
     "name": "Alice",
     "age": 30
   }
   """;

> You cannot use @ before triple quotes.
> You can use $ before triple quotes to enable string interpolation. Example:




3. At Sign: @" ... "
   What is it called?
   This is called a verbatim string literal.

   When to use it?
   Use it when you:

   Want to write multi-line strings the old way (before C# 11).
   Don’t want to escape backslashes (useful for file paths).
   Escape sequences (like \n, \t) are NOT processed, except for double quotes which you escape as "".

example :
    var path = @"C:\Users\level1cr3\Documents";
   var multiLine = @"First line
   Second line";



   ### Quick Summary Table

   | Syntax         | Name                       | Multi-line | Interpolation | Escaping      | When to use?                          |
   |----------------|---------------------------|------------|---------------|---------------|----------------------------------------|
   | `"..."`        | Regular string             | No         | No            | Yes           | Basic strings                          |
   | `$"..."`       | Interpolated string        | No         | Yes           | Yes           | Embed variables                        |
   | `@"..."`       | Verbatim string            | Yes        | No            | No            | Multi-line, file paths                 |
   | `$@"..."`      | Interpolated verbatim      | Yes        | Yes           | No            | Multi-line with variables              |
   | `"""..."""`    | Raw string literal         | Yes        | No            | No escaping   | Multi-line, lots of quotes/backslashes |
   | `$"""..."""`   | Interpolated raw string    | Yes        | Yes           | No escaping   | Multi-line with variables              |

   ---

   **Summary:**

   - Use `""" ... """` (raw string literal) for multi-line, copy-paste-friendly, no-escaping strings. Use `$""" ... """` for interpolation.
   - Use `@" ... "` for multi-line or file paths (older style). Use `$@" ... "` for interpolation with verbatim.
   - Use `$" ... "` for short strings with interpolation.
   - Only `$` can go before triple quotes, not `@`.

   Let me know if you want code samples for a specific scenario!




####### dapper functions

| Dapper Method           | Use When                                        |
| ----------------------- | ----------------------------------------------- |
| `ExecuteAsync`          | Run a command, no return (e.g. INSERT, UPDATE)  |
| `ExecuteScalarAsync<T>` | Get a **single value** (e.g. COUNT, ID, scalar) |
| `QueryAsync<T>`         | Get a list or multiple rows                     |
| `QueryFirstAsync<T>`    | Get the first row                               |
| `QuerySingleAsync<T>`   | Expect exactly one row (throws if 0 or >1)      |


 */
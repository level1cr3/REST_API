﻿using Dapper;

namespace Movies.Application.Database;

public sealed class DbInitializer(IDbConnectionFactory connectionFactory)
{
    public async Task InitializeAsync()
    {
        using var connection = await connectionFactory.CreateConnectionAsync();

        const string createMoviesTableSql = """
                                             create table if not exists movies(
                                                 id uuid primary key,
                                                 slug text not null,
                                                 title text not null,
                                                 yearofrelease integer not null
                                             );
                                            """;
        
        const string createSlugIndexSql = """
                                           create unique index concurrently if not exists movies_slug_idx
                                           on movies
                                           using btree(slug);
                                          """;

        const string createGenreTableSql = """
                                            create table if not exists genres(
                                              movieid uuid references movies (id),
                                              name text not null
                                            );
                                           """;

        const string createRatingsTableSql ="""
                                            create table if not exists ratings(
                                                userid uuid,
                                                movieid uuid references movies (id),
                                                rating integer not null,
                                                primary key (userid,movieid)
                                            );
                                            """; 
        // in ratings table we have a composite key. because primary key is composed of 2 columns.
        
        await connection.ExecuteAsync(createMoviesTableSql);
        await connection.ExecuteAsync(createSlugIndexSql);
        await connection.ExecuteAsync(createGenreTableSql);
        await connection.ExecuteAsync(createRatingsTableSql);
    }
}


/*
##### with command defination
await connection.ExecuteAsync(new CommandDefinition("""
                                                     create table if not exists movies(
                                                         id UUID primary key,
                                                         slug TEXT not null,
                                                         title TEXT not null,
                                                         yearofrelease integer not null
                                                     );
                                                     """));

* use this one if you want to pass cancellation token, or commandflags


##### transaction is not needed here it is just for example and documentation.

using var connection = await connectionFactory.CreateConnectionAsync();
using var transaction = connection.BeginTransaction();

try
{
    const string createMoviesTableSql = """
                                         create table if not exists movies(
                                             id UUID primary key,
                                             slug TEXT not null,
                                             title TEXT not null,
                                             yearofrelease integer not null
                                         );
                                        """;

    await connection.ExecuteAsync(createMoviesTableSql,transaction: transaction);

    const string createSlugIndexSql = """
                                       create unique index if not exists movies_slug_idx
                                       on movies
                                       using btree(slug);
                                      """;

    await connection.ExecuteAsync(createSlugIndexSql,transaction: transaction);


    transaction.Commit();
}
catch (Exception e)
{
    transaction.Rollback();
    Console.WriteLine($"Rolled back: {e.Message}");
}
}



##### with CONCURRENTLY  
CONCURRENTLY is used to avoid table locks during index creation in PostgreSQL, making it safer to add indexes on tables in production systems.

#### without concurrently 
you system will be locked and won't allow
No writes (INSERT/UPDATE/DELETE) can happen on the table until the index creation is finished.

*/
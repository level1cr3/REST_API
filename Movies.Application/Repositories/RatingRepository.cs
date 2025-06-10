using Dapper;
using Movies.Application.Database;

namespace Movies.Application.Repositories;

public class RatingRepository(IDbConnectionFactory connectionFactory) : IRatingRepository
{
    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        const string getMovieRatingSql = """
                                         select round(avg(rating),1) from ratings
                                         where movieid = @movieId
                                         """;

        return await connection.ExecuteScalarAsync<float?>(new CommandDefinition(getMovieRatingSql, new { movieId },
            cancellationToken: cancellationToken));
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId,
        CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        const string getMovieAndUserRatingSql = """
                                                     select round(avg(rating), 1) as rating,
                                                            max(case when userid = @userId then rating end) as userrating
                                                     from ratings
                                                     where movieid = @movieId
                                                """;

        return await connection.QuerySingleOrDefaultAsync<(float? Rating, int? UserRating)>(new CommandDefinition(
            getMovieAndUserRatingSql,
            new { movieId, userId },
            cancellationToken: cancellationToken
        ));
    }

    public async Task<bool> RateMovieAsync(Guid userId, Guid movieId, int rating,
        CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        const string upsertMovieRatingSql = """
                                            insert into ratings(userid, movieid, rating) 
                                            values (@userId, @movieId, @rating)
                                            on conflict (userid,movieid) do update 
                                            set rating = @rating
                                            """;

        var result = await connection.ExecuteAsync(new CommandDefinition(upsertMovieRatingSql,
            new { userId, movieId, rating },
            cancellationToken: cancellationToken));

        return result > 0;
    }

    public async Task<bool> DeleteRatingAsync(Guid userId, Guid movieId, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        const string deleteRatingSql = """
                                       delete from ratings 
                                       where userid = @userId and movieid = @movieId
                                       """;

        var result = await connection.ExecuteAsync(new CommandDefinition(deleteRatingSql, new { userId, movieId },
            cancellationToken: cancellationToken));

        return result > 0;
    }
    
    
}
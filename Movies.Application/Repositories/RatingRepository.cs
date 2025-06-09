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
}
﻿using FluentValidation;
using FluentValidation.Results;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class RatingService(IRatingRepository ratingRepository, IMovieRepository movieRepository) : IRatingService
{
    public async Task<bool> RateMovieAsync(Guid userId, Guid movieId, int rating, CancellationToken cancellationToken = default)
    {
        if (rating is <= 0 or > 5)
        {
            throw new ValidationException([
                new ValidationFailure{ PropertyName = "Rating", ErrorMessage = "Rating must be between 1 and 5"}
            ]);
        }

        var movieExists = await movieRepository.ExistsByIdAsync(movieId, cancellationToken);

        if (!movieExists)
        {
            return false;
        }

        return await ratingRepository.RateMovieAsync(userId, movieId, rating, cancellationToken);
    }

    public async Task<bool> DeleteRatingAsync(Guid userId, Guid movieId, CancellationToken cancellationToken = default)
    {
        return await ratingRepository.DeleteRatingAsync(userId, movieId, cancellationToken);
    }

    public async Task<IEnumerable<MovieRating>> GetAllRatingsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await ratingRepository.GetAllRatingsForUserAsync(userId, cancellationToken);
    }
}
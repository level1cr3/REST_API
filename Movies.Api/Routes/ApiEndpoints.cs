namespace Movies.Api.Routes;

public static class ApiEndpoints
{
    // all the endpoints will be defined here in a central location. because splitting endpoints in controller, method can lead to failure and hard to maintain.

    private const string
        ApiBase = "api"; // it is good to have "api" as base point. Because it can me mapped in many ways load balancers can use it exclude things.

    public static class Movies
    {
        private const string Base = $"{ApiBase}/Movies";

        public const string Create = Base;

        public const string
            Get = $"{Base}/{{idOrSlug}}"; // using double {{ to escape one. this is route parameter. using constraint so only guid can be used.

        public const string GetAll = Base;
        public const string Update = $"{Base}/{{id:guid}}";
        public const string Delete = $"{Base}/{{id:guid}}";
        
        // rating are directly attached to the movies they are not independent resource. We are going to get the ratings for the movie
        // that is why they will live inside the movies.

        public const string Rate = $"{Base}/{{id:guid}}/ratings"; // create rating for single movie.
        public const string DeleteRating = $"{Base}/{{id:guid}}/ratings"; // delete rating for single movie.
        
    }
    
    public static class Ratings
    {
        // to get all the ratings for a user
        // this is different and should live in ratings
        
        private const string Base = $"{ApiBase}/ratings"; // here ratings are the main resource.

        public const string GetUserRatings = $"{Base}/me"; // get my ratings.
        
        // this 'me' wording is very common in the rest api. It is used when you want to get something about the logged-in user.



    }
}
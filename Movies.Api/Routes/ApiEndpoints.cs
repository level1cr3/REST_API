namespace Movies.Api.Routes;

public static class ApiEndpoints
{
    // all the endpoints will be defined here in a central location. because splitting endpoints in controller, method can lead to failure and hard to maintain.
    
    private const string ApiBase = "api"; // it is good to have "api" as base point. Because it can me mapped in many ways load balancers can use it exclude things.
    
    public static class Movies
    {
        private const string Base = $"{ApiBase}/Movies";

        public const string Create = Base;
    }

}
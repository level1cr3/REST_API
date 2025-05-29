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
            Get = $"{Base}/{{id:guid}}"; // using double {{ to escape one. this is route parameter. using constraint so only guid can be used.

        public const string GetAll = Base;
        public const string Update = $"{Base}/{{id:guid}}";
        public const string Delete = $"{Base}/{{id:guid}}";
    }
}
namespace Movies.Sdk;

public static class ApiEndpoints
{
    private const string
        ApiBase = "/api";

    public static class V1
    {
        private const string VersionBase = $"{ApiBase}/v1";

        public static class Movies
        {
            private const string Base = $"{VersionBase}/Movies";

            public const string Create = Base;

            public const string
                Get = $"{Base}/{{idOrSlug}}"; 

            public const string GetAll = Base;
            public const string Update = $"{Base}/{{id}}";
            public const string Delete = $"{Base}/{{id}}";

            public const string Rate = $"{Base}/{{id}}/ratings";
            public const string DeleteRating = $"{Base}/{{id}}/ratings";
        }

        public static class Ratings
        {
            private const string Base = $"{VersionBase}/ratings";

            public const string GetUserRatings = $"{Base}/me";
        }
    }
}
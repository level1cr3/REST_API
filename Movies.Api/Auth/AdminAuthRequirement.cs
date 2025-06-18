using Microsoft.AspNetCore.Authorization;

namespace Movies.Api.Auth;

public record AdminAuthRequirement(string ApiKey) : IAuthorizationRequirement;
namespace Movies.Api.Constants;

public static class AuthConstants
{
    public const string AdminUserPolicyName = "Admin";
    
    public const string AdminUserClaimName = "admin";

    public const string TrustedOrAdminUserPolicyName = "TrustedMemberOrAdmin";
    
    public const string TrustedUserClaimName = "trusted_member";

    public const string ApiKeyHeaderName = "x-api-key";
}
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;

namespace Movies.Sdk.Consumer;

public class AuthTokenProvider(HttpClient httpClient)
{
    private string _cachedToken = string.Empty;

    private static readonly SemaphoreSlim Lock = new(1, 1);
    // to handle thread safety in async context.
    // to make sure there is only one request at a time. when we generate a token

    public async Task<string> GetTokenAsync()
    {
        if (!string.IsNullOrWhiteSpace(_cachedToken) && !IsTokenExpired(_cachedToken))
        {
            return _cachedToken;
        }

        await Lock.WaitAsync();

        try
        {
            // this condition is written twice. since lock will allow only one request at a time. 2nd or 3rd request would get returned from here.
            if (!string.IsNullOrWhiteSpace(_cachedToken) && !IsTokenExpired(_cachedToken))
            {
                return _cachedToken;
            }

            var response = await httpClient.PostAsJsonAsync("https://localhost:5003/token", new
            {
                userId = "d8566de3-b1a6-4a9b-b842-8e3887a82e42",
                email = "seucu@nickchapsas.com",
                customClaims = new Dictionary<string, object>
                {
                    { "admin", true },
                    { "trusted_member", true }
                }
            });

            var newToken = await response.Content.ReadAsStringAsync();
            _cachedToken = newToken;
        }
        finally
        {
            Lock.Release();
        }

        return _cachedToken;
    }

    private static bool IsTokenExpired(string cacheToken)
    {
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(cacheToken);
        var expireTimeTxt = jwt.Claims.Single(claim => claim.Type == "exp").Value;
        var expiryDateTime = UnixTimeStampToDateTime(int.Parse(expireTimeTxt));
        return expiryDateTime < DateTime.UtcNow;
    }

    private static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }
}


/*

// in above code it is okay since we are the only user here no other will call this.


 ✅ So what should you do instead?
Use a separate lock per user or resource, if needed.

🔹 Example — lock per user:

private static readonly ConcurrentDictionary<string, SemaphoreSlim> UserLocks = new();

public async Task DoSomething(string userId)
{
    var userLock = UserLocks.GetOrAdd(userId, _ => new SemaphoreSlim(1, 1));

    await userLock.WaitAsync();
    try
    {
        // Critical section just for this user
    }
    finally
    {
        userLock.Release();
    }
}
🟢 Now, each user gets their own lock, and they don't block each other.




| Situation                  | What Happens                   | Is It Good?                 |
| -------------------------- | ------------------------------ | --------------------------- |
| One global `SemaphoreSlim` | All users wait one-by-one      | ❌ Slow, bad for performance |
| Lock per user/resource     | Only that user’s requests wait | ✅ Fast and safe             |


 */
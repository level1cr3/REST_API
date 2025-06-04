using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api.Controllers;

// Identity.Api is fake service for generating JWT token.
// When we are processing the JWT Token in REST API we don't necessary know where they are generated from all we need to do is validate the signature

[ApiController]
public class IdentityController : ControllerBase
{
    private const string TokenSecret = "ForLoveOfGodStoreAndLoadThisSecurely";
    private static readonly TimeSpan TokenLifeTime = TimeSpan.FromHours(8);

    [HttpPost("token")]
    public IActionResult GenerateToken([FromBody] TokenGenerationRequest request)
    {
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, request.Email),
            new(JwtRegisteredClaimNames.Email, request.Email),
            new("userid", request.UserId.ToString())
        ];

        foreach (var claimPair in request.CustomClaims)
        {
            var jsonElement = (JsonElement)claimPair.Value;

            // test this. this also works
            // var jsonElement2 = claimPair.Value switch
            // {
            //     JsonElement json => json,
            //     _ => throw new BadHttpRequestException("Invalid claim value format")
            // };

            var valueType = jsonElement.ValueKind switch
            {
                JsonValueKind.True => ClaimValueTypes.Boolean,
                JsonValueKind.False => ClaimValueTypes.Boolean,
                JsonValueKind.Number => ClaimValueTypes.Double,
                _ => ClaimValueTypes.String
            };

            var claim = new Claim(claimPair.Key, claimPair.Value.ToString(), valueType);

            claims.Add(claim);
        }


        var key = Encoding.UTF8.GetBytes(TokenSecret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(TokenLifeTime),
            Issuer = "https://id.example.com",
            Audience = "https://id.example.com",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature )
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        return Ok(jwt);
    }
}


/*
jti
    stands for JWT ID — a unique identifier for the token.

Purpose of jti:
 Prevents replay attacks by making each token uniquely identifiable.
 Can be used in a token blacklist or revocation list.
 Helps with logging/debugging individual tokens.

Sub : is subject
    The sub claim is meant to identify the principal (i.e., the user or entity) the JWT is issued for.
    Identifier of the user the token is for (often user ID, username, or email).


*/
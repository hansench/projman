using ProjMan.Infrastructure.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ProjMan.Infrastructure.Security;

public class TokenGenerator : ITokenGenerator
{
    private readonly JwtIssuerOptions _jwtOptions;

    public TokenGenerator(IOptions<JwtIssuerOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }


    public async Task<string> CreateJwtToken(int userId, string userName, int roleId, string fullName)
    {
        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(_jwtOptions.IssuedAt).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(CustomClaimNames.UserId, userId.ToString()),
                new Claim(CustomClaimNames.UserName, userName),
                new Claim(CustomClaimNames.FullName, fullName),
                new Claim(CustomClaimNames.RoleId, roleId.ToString()),
            };
        var jwt = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            _jwtOptions.NotBefore,
            _jwtOptions.Expiration,
            _jwtOptions.SigningCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }


    public string CreateRefreshToken(int size = 32)
    {
        var randomNumber = new byte[size];
        using var numberGenerator = RandomNumberGenerator.Create();
        numberGenerator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}


public interface ITokenGenerator
{
    Task<string> CreateJwtToken(int userId, string userName, int roleId, string fullName);

    string CreateRefreshToken(int size = 32);
}

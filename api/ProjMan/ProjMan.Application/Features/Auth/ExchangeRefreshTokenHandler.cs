using Microsoft.IdentityModel.Tokens;
using ProjMan.Application.Features.Auth.Dtos;
using ProjMan.Application.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProjMan.Application.Features.Auth;

public class ExchangeRefreshTokenHandler : IRequestHandler<ExchangeRefreshTokenRequest, RowResponse<LoginUserDto>>
{
    private readonly IAuthRepository _repository;
    private readonly JwtSettings _jwtSettings;

    public ExchangeRefreshTokenHandler(IAuthRepository repository, IOptions<JwtSettings> options)
    {
        _repository = repository;
        _jwtSettings = options.Value;
    }

    public async Task<RowResponse<LoginUserDto>> Handle(ExchangeRefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var user = GetIdentifierFromExpiredToken(request.Token);
        if (user == null) 
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        return await _repository.ExchangeRefreshTokenAsync(user.Value, request.RefreshToken);
    }


    private Claim GetIdentifierFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _jwtSettings.SigningCredentials?.Key ?? throw new ArgumentNullException(),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = false, // do not check for expiry date time
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(
            token,
            tokenValidationParameters,
            out var securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Contains(
                SecurityAlgorithms.HmacSha256Signature,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
    }
}

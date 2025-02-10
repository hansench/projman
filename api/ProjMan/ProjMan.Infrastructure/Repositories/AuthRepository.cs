using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProjMan.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ProjManDbContext _dbContext;
    private readonly RefreshTokenSettings _refreshTokenSettings;
    private readonly JwtSettings _jwtSettings;

    public AuthRepository(ProjManDbContext dbContext,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator,
        IOptions<RefreshTokenSettings> options1,
        IOptions<JwtSettings> options2)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _refreshTokenSettings = options1.Value;
        _jwtSettings = options2.Value;
    }

    public async Task<RowResponse<LoginUserDto>> LoginAsync(string username, string password)
    {
        var response = new RowResponse<LoginUserDto>();

        var messageInvalid = "Invalid credentials.";
        username = username.Trim().ToLowerInvariant();

        try
        {
            var user = await _dbContext.AppUserDbSet
                .SingleOrDefaultAsync(x => x.UserName == username && x.IsActive);

            if (user == null)
            {
                throw new UnauthorizedAccessException(messageInvalid);
            }

            if (user.Hashed == null || user.Salted == null)
            {
                throw new UnauthorizedAccessException(messageInvalid);
            }

            if (!user.Hashed.SequenceEqual(_passwordHasher.Hash(password, user.Salted)))
            {
                throw new UnauthorizedAccessException(messageInvalid);
            }

            ClearRefreshToken(user.Id);

            // generate refresh token
            var refreshToken = _tokenGenerator.CreateRefreshToken();
            await AddRefreshToken(user.Id, refreshToken);
            await _dbContext.SaveChangesAsync();

            var token = await _tokenGenerator.CreateJwtToken(user.Id, user.UserName, user.RoleId, user.FullName);

            var authorizedUser = new LoginUserDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Token = token,
                RefreshToken = refreshToken
            };

            response.Ok = true;
            response.Row = authorizedUser;
            response.Message = string.Empty;
        }
        catch (Exception ex)
        {
            response.Ok = false;
            response.Status = StatusCodes.Status401Unauthorized.ToString();
            response.Message = ex.Message;
        }

        return response;
    }


    public async Task<RowResponse<LoginUserDto>> ExchangeRefreshTokenAsync(string token, string refreshToken)
    {
        var response = new RowResponse<LoginUserDto>();

        var userClaim = GetIdentifierFromExpiredToken(token);
        if (userClaim == null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var username = userClaim.Value.Trim().ToLowerInvariant();

        try
        {
            var user = await _dbContext.AppUserDbSet
                .SingleAsync(x => x.UserName == username && x.IsActive);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            if (!(await IsValidRefreshToken(user.Id, refreshToken)))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            ClearRefreshToken(user.Id);

            var newRefreshToken = _tokenGenerator.CreateRefreshToken();
            await AddRefreshToken(user.Id, newRefreshToken);
            await _dbContext.SaveChangesAsync();

            var newtoken = await _tokenGenerator.CreateJwtToken(user.Id, user.UserName, user.RoleId, user.FullName);

            var authorizedUser = new LoginUserDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Token = newtoken,
                RefreshToken = refreshToken
            };

            response.Ok = true;
            response.Row = authorizedUser;
            response.Message = string.Empty;
        }
        catch (Exception ex)
        {
            response.Ok = false;
            response.Status = StatusCodes.Status401Unauthorized.ToString();
            response.Message = ex.Message;
        }

        return response;
    }

    
    private async Task<bool> IsValidRefreshToken(int userId, string token)
    {
        var refreshToken = await _dbContext.AppUserRefreshTokenDbSet
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Token == token && x.IsActive);

        if (refreshToken == null) return false;

        return refreshToken.ExpiredUtc > DateTime.UtcNow;
    }


    private async Task AddRefreshToken(int userId, string token)
    {
        var refreshToken = new AppUserRefreshTokenEntity
        {
            Token = token,
            UserId = userId,
            ExpiredUtc = DateTime.UtcNow.AddMinutes(_refreshTokenSettings.ValidMinutes),
            InsertedUtc = DateTime.UtcNow
        };

        await _dbContext.AppUserRefreshTokenDbSet.AddAsync(refreshToken);
    }


    private void ClearRefreshToken(int userId)
    {
        var refreshTokens = _dbContext.AppUserRefreshTokenDbSet
            .Where(x => x.UserId == userId);

        foreach (var refreshToken in refreshTokens)
        {
            _dbContext.AppUserRefreshTokenDbSet.Remove(refreshToken);
        }
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

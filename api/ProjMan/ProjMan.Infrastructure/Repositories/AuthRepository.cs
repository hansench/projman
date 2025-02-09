using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProjMan.Application.Features.Auth.Dtos;
using ProjMan.Application.Security;
using ProjMan.Infrastructure.Database.Entities;

namespace ProjMan.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ProjManDbContext _dbContext;
    private readonly RefreshTokenSettings _refreshTokenSettings;

    public AuthRepository(ProjManDbContext dbContext, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator, IOptions<RefreshTokenSettings> options)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _refreshTokenSettings = options.Value;
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


    public async Task<RowResponse<LoginUserDto>> ExchangeRefreshTokenAsync(string username, string refreshToken)
    {
        var response = new RowResponse<LoginUserDto>();

        try
        {
            username = username.Trim().ToLowerInvariant();
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

            var newRefreshToken = _tokenGenerator.CreateRefreshToken();
            await RemoveRefreshToken(user.Id, refreshToken);
            await AddRefreshToken(user.Id, newRefreshToken);
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


    private async Task RemoveRefreshToken(int userId, string token)
    {
        var refreshToken = await _dbContext.AppUserRefreshTokenDbSet
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Token == token);

        if (refreshToken == null) return;

        _dbContext.AppUserRefreshTokenDbSet.Remove(refreshToken);
    }
}

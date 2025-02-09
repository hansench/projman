using ProjMan.Application.Features.Auth.Dtos;

namespace ProjMan.Application.Interfaces;

public interface IAuthRepository
{
    Task<RowResponse<LoginUserDto>> LoginAsync(string username, string password);

    Task<RowResponse<LoginUserDto>> ExchangeRefreshTokenAsync(string username, string refreshToken);
}

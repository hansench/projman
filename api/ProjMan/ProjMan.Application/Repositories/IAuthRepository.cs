namespace ProjMan.Application.Repositories;

public interface IAuthRepository
{
    Task<RowResponse<LoginUserDto>> LoginAsync(string username, string password);

    Task<RowResponse<LoginUserDto>> ExchangeRefreshTokenAsync(string username, string refreshToken);
}

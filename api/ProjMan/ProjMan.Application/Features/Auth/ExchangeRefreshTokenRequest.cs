using ProjMan.Application.Features.Auth.Dtos;

namespace ProjMan.Application.Features.Auth;

public class ExchangeRefreshTokenRequest(string token, string refreshToken) : IRequest<RowResponse<LoginUserDto>>
{
    public string Token { get; set; } = token;
    public string RefreshToken { get; set; } = refreshToken;
}


public class ExchangeRefreshTokenRequestValidator : AbstractValidator<ExchangeRefreshTokenRequest>
{
    public ExchangeRefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

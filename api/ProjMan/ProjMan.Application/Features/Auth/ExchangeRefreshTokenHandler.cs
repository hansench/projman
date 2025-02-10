namespace ProjMan.Application.Features.Auth;

public class ExchangeRefreshTokenHandler : IRequestHandler<ExchangeRefreshTokenRequest, RowResponse<LoginUserDto>>
{
    private readonly IAuthRepository _repository;

    public ExchangeRefreshTokenHandler(IAuthRepository repository)
    {
        _repository = repository;
    }

    public async Task<RowResponse<LoginUserDto>> Handle(ExchangeRefreshTokenRequest request, CancellationToken cancellationToken)
    {
        return await _repository.ExchangeRefreshTokenAsync(request.Token, request.RefreshToken);
    }
}

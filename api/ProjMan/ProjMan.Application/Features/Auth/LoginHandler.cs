using ProjMan.Application.Features.Auth.Dtos;

namespace ProjMan.Application.Features.Auth;

public class LoginHandler : IRequestHandler<LoginRequest, RowResponse<LoginUserDto>>
{
    private readonly IAuthRepository _repository;

    public LoginHandler(IAuthRepository repository)
    {
        _repository = repository;
    }

    public async Task<RowResponse<LoginUserDto>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        return await _repository.LoginAsync(request.UserName, request.Password);
    }
}

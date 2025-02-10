namespace ProjMan.Application.Features.Auth;

public class LoginRequest(string username, string password) : IRequest<RowResponse<LoginUserDto>>
{
    public string UserName { get; set; } = username;
    public string Password { get; set; } = password;
}


public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

using ProjMan.Application.Features.Auth;

namespace ProjMan.Controllers;

[Route("api/auth")]
public class AuthController : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        return Result(await Mediator.Send(request));
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] ExchangeRefreshTokenRequest request)
    {
        return Result(await Mediator.Send(request));
    }
}

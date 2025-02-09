namespace ProjMan.Controllers;

[Route("start")]
[ApiController]
[AllowAnonymous]
public class StartController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Oke");
    }
}

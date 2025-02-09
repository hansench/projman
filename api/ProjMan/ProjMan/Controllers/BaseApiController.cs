namespace ProjMan.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    private ISender _mediator = null!;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();


    protected IActionResult Result(BaseResponse response)
    {
        if (response.Ok)
        {
            return Ok(response);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(response.Status))
            {
                return BadRequest(response);
            }

            return StatusCode(response.Status.ToInt());
        }
    }
}

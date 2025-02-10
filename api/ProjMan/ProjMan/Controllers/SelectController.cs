using ProjMan.Application.Features.SelectList;

namespace ProjMan.Controllers;

[Route("api/select")]
public class SelectController : BaseApiController
{
    [HttpGet("stage")]
    public async Task<IActionResult> StageSelectList()
    {
        return Result(await Mediator.Send(new StageSelectListRequest()));
    }

    [HttpGet("category")]
    public async Task<IActionResult> CategorySelectList()
    {
        return Result(await Mediator.Send(new CategorySelectListRequest()));
    }
}

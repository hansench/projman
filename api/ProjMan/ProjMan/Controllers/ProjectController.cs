using ProjMan.Application.Features.ProjectFetch;
using ProjMan.Application.Features.ProjectInsert;
using ProjMan.Application.Features.ProjectPagedList;
using ProjMan.Application.Features.ProjectUpdate;

namespace ProjMan.Controllers;

[Route("api/project")]
public class ProjectController : BaseApiController
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Fetch([FromRoute] int id)
    {
        return Result(await Mediator.Send(new ProjectFetchRequest(id)));
    }

    [HttpPost("list")]
    public async Task<IActionResult> PagedList([FromBody] ProjectPagedListRequest request)
    {
        return Result(await Mediator.Send(request));
    }

    [HttpPost]
    public async Task<IActionResult> Insert([FromBody] ProjectInsertRequest request)
    {
        return Result(await Mediator.Send(request));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ProjectUpdateRequest request)
    {
        return Result(await Mediator.Send(request));
    }
}

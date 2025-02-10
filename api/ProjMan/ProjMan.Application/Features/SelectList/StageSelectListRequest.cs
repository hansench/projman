using ProjMan.Application.Behavior;

namespace ProjMan.Application.Features.SelectList;

public class StageSelectListRequest : IRequest<ListResponse<SelectItemDto>>, ICacheable
{
    public bool BypassCache => false;

    public string CacheKey => $"select/stage";

    public int SlidingExpirationInMinutes => 30;

    public int AbsoluteExpirationInMinutes => 600;
}

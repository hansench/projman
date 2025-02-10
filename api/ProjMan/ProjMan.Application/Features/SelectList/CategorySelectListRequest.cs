using ProjMan.Application.Behavior;

namespace ProjMan.Application.Features.SelectList;

public class CategorySelectListRequest : IRequest<ListResponse<SelectItemDto>>, ICacheable
{
    public bool BypassCache => false;

    public string CacheKey => $"select/category";

    public int SlidingExpirationInMinutes => 30;

    public int AbsoluteExpirationInMinutes => 600;
}

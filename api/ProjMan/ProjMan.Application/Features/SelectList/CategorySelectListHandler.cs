namespace ProjMan.Application.Features.SelectList;

public class CategorySelectListHandler : IRequestHandler<CategorySelectListRequest, ListResponse<SelectItemDto>>
{
    private readonly ISelectRepository _repository;

    public CategorySelectListHandler(ISelectRepository repository)
    {
        _repository = repository;
    }

    public async Task<ListResponse<SelectItemDto>> Handle(CategorySelectListRequest request, CancellationToken cancellationToken)
    {
        return await _repository.FetchCategorySelectList();
    }
}

namespace ProjMan.Application.Features.SelectList;

public class StageSelectListHandler : IRequestHandler<StageSelectListRequest, ListResponse<SelectItemDto>>
{
    private readonly ISelectRepository _repository;

    public StageSelectListHandler(ISelectRepository repository)
    {
        _repository = repository;
    }

    public async Task<ListResponse<SelectItemDto>> Handle(StageSelectListRequest request, CancellationToken cancellationToken)
    {
        return await _repository.FetchStageSelectList();
    }
}

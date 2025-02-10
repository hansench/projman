namespace ProjMan.Application.Features.ProjectPagedList;

public class ProjectPagedListHandler : IRequestHandler<ProjectPagedListRequest, PagedListResponse<ProjectInfoDto>>
{
    private readonly IProjectRepository _repository;

    public ProjectPagedListHandler(IProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedListResponse<ProjectInfoDto>> Handle(ProjectPagedListRequest request, CancellationToken cancellationToken)
    {
        return await _repository.FetchPagedListAsync(request);
    }
}

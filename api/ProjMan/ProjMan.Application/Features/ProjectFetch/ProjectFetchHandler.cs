namespace ProjMan.Application.Features.ProjectFetch;

public class ProjectFetchHandler : IRequestHandler<ProjectFetchRequest, RowResponse<ProjectUpdateDto>>
{
    private readonly IProjectRepository _repository;

    public ProjectFetchHandler(IProjectRepository repository)
    {
        _repository = repository;
    }


    public async Task<RowResponse<ProjectUpdateDto>> Handle(ProjectFetchRequest request, CancellationToken cancellationToken)
    {
        return await _repository.FetchAsync(request.Id);
    }
}

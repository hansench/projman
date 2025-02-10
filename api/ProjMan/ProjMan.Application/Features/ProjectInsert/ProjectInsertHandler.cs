namespace ProjMan.Application.Features.ProjectInsert;

public class ProjectInsertHandler : IRequestHandler<ProjectInsertRequest, RowResponse<ProjectUpdateDto>>
{
    private readonly IProjectRepository _repository;

    public ProjectInsertHandler(IProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<RowResponse<ProjectUpdateDto>> Handle(ProjectInsertRequest request, CancellationToken cancellationToken)
    {
        return await _repository.InsertAsync(request);
    }
}

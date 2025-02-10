namespace ProjMan.Application.Features.ProjectUpdate;

internal class ProjectUpdateHandler : IRequestHandler<ProjectUpdateRequest, RowResponse<ProjectUpdateDto>>
{
    private readonly IProjectRepository _repository;

    public ProjectUpdateHandler(IProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<RowResponse<ProjectUpdateDto>> Handle(ProjectUpdateRequest request, CancellationToken cancellationToken)
    {
        return await _repository.UpdateAsync(request);
    }
}

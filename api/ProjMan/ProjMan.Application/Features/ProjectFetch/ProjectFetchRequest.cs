namespace ProjMan.Application.Features.ProjectFetch;

public class ProjectFetchRequest : IRequest<RowResponse<ProjectUpdateDto>>
{
    public ProjectFetchRequest(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}


public class ProjectFetchRequestValidator : AbstractValidator<ProjectFetchRequest>
{
    public ProjectFetchRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

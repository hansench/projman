namespace ProjMan.Application.Features.ProjectUpdate;

public class ProjectUpdateRequest : ProjectUpdateDto, IRequest<RowResponse<ProjectUpdateDto>>
{
}


public class ProjectUpdateRequestValidator : AbstractValidator<ProjectUpdateRequest>
{
    public ProjectUpdateRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ProjectName).NotEmpty().MinimumLength(3).MaximumLength(200);
        RuleFor(x => x.ProjectLocation).NotEmpty().MinimumLength(3).MaximumLength(500);
        RuleFor(x => x.ProjectDetails).NotEmpty().MinimumLength(3).MaximumLength(2000);
        RuleFor(x => x.StageId).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();

        When(x => x.CategoryId == 4, () =>
        {
            RuleFor(x => x.CategoryOthersDescr).NotEmpty().MinimumLength(3).MaximumLength(200);
        });

        RuleFor(x => x.StartDate).NotEmpty();
        When(x => x.StageId != 4, () =>
        {
            RuleFor(x => x.StartDate).GreaterThanOrEqualTo(DateTime.Now.Date);
        });
    }
}
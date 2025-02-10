namespace ProjMan.Application.Dtos;

public class ProjectUpdateDto
{
    public string ProjectName { get; set; } = string.Empty;

    public string ProjectLocation { get; set; } = string.Empty;

    public string ProjectDetails { get; set; } = string.Empty;

    public int StageId { get; set; }

    public int CategoryId { get; set; }

    public string CategoryOthersDescr { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public int Id { get; set; }
}

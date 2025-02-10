namespace ProjMan.Infrastructure.Database.Entities;

public class ProjectEntity : BaseEntity<int>
{
    public string ProjectName { get; set; } = string.Empty;

    public string ProjectLocation { get; set; } = string.Empty;

    public string ProjectDetails {  get; set; } = string.Empty;

    public int StageId { get; set; }

    public int CategoryId { get; set; }

    public string CategoryOthersDescr { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }
}

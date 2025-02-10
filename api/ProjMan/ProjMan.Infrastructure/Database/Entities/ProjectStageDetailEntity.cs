namespace ProjMan.Infrastructure.Database.Entities;

public class ProjectStageDetailEntity : BaseEntity<int>
{
    public int ProjectId { get; set; }

    public int StageId { get; set; }

    public string Remarks { get; set; } = string.Empty;
}

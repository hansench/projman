using ProjMan.Application.Attributes;

namespace ProjMan.Application.Dtos;

public class ProjectInfoDto
{
    public int Id { get; set; }

    [Searchable]
    public string ProjectName { get; set; } = string.Empty;

    public int StageId { get; set; }

    [Searchable]
    public string StageName { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    [Searchable]
    [JsonIgnore]
    public string CategoryName { get; set; } = string.Empty;

    [JsonIgnore]
    public string CategoryOthersDescr { get; set; } = string.Empty;

    public string CategoryDescr
    {
        get
        {
            if (!string.IsNullOrEmpty(CategoryOthersDescr))
            {
                return $"{CategoryName}: {CategoryOthersDescr}";
            }

            return CategoryName;
        }
    }

    [JsonIgnore]
    public DateTime StartDate { get; set; }

    public string StartDateString
    {
        get
        {
            return StartDate.ToString("yyyy-MM-dd");
        }
    }
}

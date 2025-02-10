namespace ProjMan.Application.Dtos;

public class PagedListRequest
{
    public int PageSize { get; set; } = 10;

    public int Page { get; set; } = 1;

    public string Search { get; set; } = string.Empty;

    public string SortField { get; set; } = string.Empty;

    public int SortOrder { get; set; }
}

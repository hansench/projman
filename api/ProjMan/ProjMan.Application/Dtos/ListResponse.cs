namespace ProjMan.Application.Dtos;

public class ListResponse<T> : BaseResponse
{
    public List<T> Data { get; set; } = new();

    public int Total { get; set; }
}

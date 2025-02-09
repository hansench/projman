namespace ProjMan.Application.Dtos;

public class RowResponse<T> : BaseResponse
{
    public T? Row { get; set; } = default!;
}

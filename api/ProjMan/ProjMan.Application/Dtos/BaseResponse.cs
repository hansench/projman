namespace ProjMan.Application.Dtos;

public class BaseResponse
{
    public bool Ok { get; set; }

    [JsonIgnore]
    public string Status { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}

namespace ProjMan.Application.Exceptions;

public class CustomValidationException : CustomException
{
    public CustomValidationException(List<string> errors)
        : base("Validation Error", errors, HttpStatusCode.BadRequest)
    {
    }
}

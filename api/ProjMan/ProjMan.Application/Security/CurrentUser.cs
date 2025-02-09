using Microsoft.AspNetCore.Http;
using ProjMan.Application.Constants;

namespace ProjMan.Application.Security;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserName
    {
        get
        {
            if (_httpContextAccessor.HttpContext is null) return string.Empty;
            if (_httpContextAccessor.HttpContext.User is null) return string.Empty;

            try
            {
                var usernameClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(CustomClaimNames.UserName, StringComparison.OrdinalIgnoreCase))!.Value;
                return usernameClaim == null ? string.Empty : usernameClaim;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }


    public int UserId
    {
        get
        {

            try
            {
                if (_httpContextAccessor.HttpContext is null) return 0;

                var claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(CustomClaimNames.UserId, StringComparison.OrdinalIgnoreCase))!.Value;
                return claim == null ? 0 : Convert.ToInt32(claim);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }


    public string FullName
    {
        get
        {

            try
            {
                if (_httpContextAccessor.HttpContext is null) return string.Empty;

                var tenantNameClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(CustomClaimNames.FullName, StringComparison.OrdinalIgnoreCase))!.Value;
                return tenantNameClaim == null ? string.Empty : tenantNameClaim;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}


public interface ICurrentUser
{
    int UserId { get; }
    string UserName { get; }
    string FullName { get; }
}

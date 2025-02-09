using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ProjMan.Application.Security;

public class JwtSettings
{
    public string SecurityKey { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public int ValidMinutes { get; set; }

    public SigningCredentials? SigningCredentials
    {
        get
        {
            if (string.IsNullOrWhiteSpace(SecurityKey)) return null;
            return new(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecurityKey)), SecurityAlgorithms.HmacSha256Signature);
        }
    }

}

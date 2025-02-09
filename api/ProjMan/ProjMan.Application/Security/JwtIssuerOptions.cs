using Microsoft.IdentityModel.Tokens;

namespace ProjMan.Application.Security;

public class JwtIssuerOptions
{
    public string Issuer { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public DateTime Expiration => IssuedAt.Add(ValidFor);

    public DateTime NotBefore => DateTime.UtcNow;

    public DateTime IssuedAt => DateTime.UtcNow;

    public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(60);


    public Func<Task<string>> JtiGenerator =>
        () => Task.FromResult(Guid.NewGuid().ToString());

    public SigningCredentials? SigningCredentials { get; set; }
}

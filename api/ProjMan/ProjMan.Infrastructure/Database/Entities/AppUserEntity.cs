namespace ProjMan.Infrastructure.Database.Entities;

public class AppUserEntity : BaseEntity<int>
{
    public string UserName { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public int RoleId { get; set; }

    public byte[]? Hashed { get; set; }

    public byte[]? Salted { get; set; }

    public bool IsActive { get; set; } = true;
}

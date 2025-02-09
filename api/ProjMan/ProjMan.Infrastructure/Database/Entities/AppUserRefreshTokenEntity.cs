using System.ComponentModel.DataAnnotations;

namespace ProjMan.Infrastructure.Database.Entities;

public class AppUserRefreshTokenEntity
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = string.Empty;

    public DateTime InsertedUtc { get; set; } = DateTime.UtcNow;

    public DateTime ExpiredUtc { get; set; }

    public bool IsActive { get; set; } = true;
}

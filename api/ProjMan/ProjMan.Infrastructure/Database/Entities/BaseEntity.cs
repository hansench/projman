using System.ComponentModel.DataAnnotations;

namespace ProjMan.Infrastructure.Database.Entities;

public abstract class BaseEntity<T>
{
    [Key]
    public T Id { get; set; } = default!;

    public int InsertedUserId { get; set; }

    public DateTime InsertedUtc { get; set; } = DateTime.UtcNow;

    public int UpdatedUserId { get; set; }

    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}

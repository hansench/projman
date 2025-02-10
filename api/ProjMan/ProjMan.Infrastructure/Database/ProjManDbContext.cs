using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ProjMan.Infrastructure.Database;

public class ProjManDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;

    public ProjManDbContext(DbContextOptions options,
        IConfiguration configuration,
        ILoggerFactory loggerFactory)
        : base(options)
    {
        _configuration = configuration;
        _loggerFactory = loggerFactory;
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
        optionsBuilder.UseLoggerFactory(_loggerFactory);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUserEntity>(e => {
            e.ToTable("AppUser");
            e.Property("Id").UseIdentityAlwaysColumn();
            e.HasIndex("UserName").IsUnique();
        });

        modelBuilder.Entity<AppUserRefreshTokenEntity>(e => {
            e.ToTable("AppUserRefreshToken");
            e.Property("Id").UseIdentityAlwaysColumn();
        });


        modelBuilder.Entity<ProjectEntity>(e => {
            e.ToTable("Project");
            e.Property("Id").UseIdentityAlwaysColumn();
            e.HasIndex("ProjectName");
        });

        modelBuilder.Entity<ProjectStageDetailEntity>(e => {
            e.ToTable("ProjectStageDetail");
            e.Property("Id").UseIdentityAlwaysColumn();
        });
    }

    public DbSet<AppUserEntity> AppUserDbSet { get; set; }

    public DbSet<AppUserRefreshTokenEntity> AppUserRefreshTokenDbSet { get; set; }

    public DbSet<ProjectEntity> ProjectDbSet { get; set; }

    public DbSet<ProjectStageDetailEntity> ProjectStageDetailDbSet { get; set; }
}

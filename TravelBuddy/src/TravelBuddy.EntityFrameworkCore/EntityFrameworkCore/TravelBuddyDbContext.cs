using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using TravelBuddy; // Para IUserOwned
using TravelBuddy.Destinos;
using TravelBuddy.Ratings;        // <-- NUEVO
using TravelBuddy.Users;          // <-- NUEVO
using TravelBuddy.Administration;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.Users;             // <-- NUEVO


namespace TravelBuddy.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ConnectionStringName("Default")]
public class TravelBuddyDbContext :
    AbpDbContext<TravelBuddyDbContext>,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    public DbSet<Destino> Destinos { get; set; }

    // NUEVO: DbSet de calificaciones
    public DbSet<DestinationRating> DestinationRatings { get; set; } = default!;

    public DbSet<ApiUsageLog> ApiUsageLogs { get; set; }
    #region Entities from the modules

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    public DbSet<Rating> Ratings { get; set; }

    #endregion


    // NUEVO: inyección de ICurrentUser (null en design-time)
    private readonly ICurrentUser? _currentUser;

    public TravelBuddyDbContext(
        DbContextOptions<TravelBuddyDbContext> options,
        ICurrentUser? currentUser = null // permite migraciones sin usuario
    ) : base(options)

    {
        _currentUser = currentUser;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);


        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureBlobStoring();

        builder.Entity<Destino>(b =>
        {
            b.ToTable("Destinos");  // ✅ Nombre directo sin prefijo
            b.ConfigureByConvention();
        });


        // NUEVO: mapeo DestinationRating
        builder.Entity<DestinationRating>(b =>
        {
            b.ToTable("DestinationRatings");
            b.HasKey(x => x.Id);
            b.Property(x => x.Score).IsRequired();
            // Si querés 1 sola calificación por (Destino, Usuario), cambiá a .IsUnique(true)
            b.HasIndex(x => new { x.DestinationId, x.UserId }).IsUnique(false);
        });

        builder.Entity<ApiUsageLog>(b =>
        {
            b.ToTable("ApiUsageLogs");
            b.ConfigureByConvention();
            b.HasIndex(x => x.CreationTime); // Indice para búsquedas rápidas por fecha
        });

        // NUEVO: aplica filtro global a todas las entidades IUserOwned

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(IUserOwned).IsAssignableFrom(entityType.ClrType))
            {

                var method = typeof(TravelBuddyDbContext)
                    .GetMethod(nameof(ApplyUserOwnedFilter),
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(this, new object[] { builder });
            }
        }
    }

    // NUEVO: HasQueryFilter(UserId == usuario actual). Si no hay usuario => 0 filas.
    private void ApplyUserOwnedFilter<TEntity>(ModelBuilder builder) where TEntity : class, IUserOwned
    {
        builder.Entity<TEntity>().HasQueryFilter(e =>
            _currentUser != null && _currentUser.IsAuthenticated
                ? e.UserId == _currentUser.GetId()
                : false
        );

    }
}

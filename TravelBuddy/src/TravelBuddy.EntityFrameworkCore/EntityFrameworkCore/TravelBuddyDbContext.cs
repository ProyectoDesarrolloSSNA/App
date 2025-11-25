using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using TravelBuddy.Destinos;
using TravelBuddy.Ratings;
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
using Volo.Abp.Users;

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
            b.ToTable("Destinos");
            b.ConfigureByConvention();
        });


        // NUEVO: mapeo DestinationRating
        builder.Entity<DestinationRating>(b =>
        {
            b.ToTable("DestinationRatings");
            b.HasKey(x => x.Id);
            b.Property(x => x.Score).IsRequired();
            b.HasIndex(x => new { x.DestinationId, x.UserId }).IsUnique(false);
        });

        // NO aplicar filtro global para IUserOwned porque manejamos la seguridad a nivel de aplicación
        // El filtro global causaba problemas en operaciones que necesitan ver todos los registros
        // como GetAverageRatingAsync y GetAllByDestinationAsync
    }
}

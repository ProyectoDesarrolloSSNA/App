using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using TravelBuddy.Destinos;
using TravelBuddy.Ratings;
using TravelBuddy.ExperienciasViaje;
using TravelBuddy.Favorites;
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
    public DbSet<Destino> Destinos { get; set; } 
    public DbSet<DestinationRating> DestinationRatings { get; set; } = default!;
    public DbSet<ExperienciaViaje> ExperienciasViaje { get; set; }
    public DbSet<DestinationFavorite> DestinationFavorites { get; set; } = default!;
    
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
        builder.ConfigureTenantManagement();

        builder.Entity<Destino>(b =>
        {
            b.ToTable("Destinos");
            b.ConfigureByConvention();
        });

        // Mapeo DestinationRating
        builder.Entity<DestinationRating>(b =>
        {
            b.ToTable("DestinationRatings");
            b.HasKey(x => x.Id);
            b.Property(x => x.Score).IsRequired();
            b.HasIndex(x => new { x.DestinationId, x.UserId }).IsUnique(false);
        });

        // Mapeo DestinationFavorite
        builder.Entity<DestinationFavorite>(b =>
        {
            b.ToTable("DestinationFavorites");
            b.HasKey(x => x.Id);
            b.Property(x => x.DestinationId).IsRequired();
            b.Property(x => x.UserId).IsRequired();
            // Índice único para evitar duplicados de favoritos por usuario y destino
            b.HasIndex(x => new { x.UserId, x.DestinationId }).IsUnique();
            b.HasIndex(x => x.DestinationId);
        });

        // NO aplicar filtro global para IUserOwned porque manejamos la seguridad a nivel de aplicación
        // El filtro global causaba problemas en operaciones que necesitan ver todos los registros
        // como GetAverageRatingAsync y GetAllByDestinationAsync
        // Mapeo ExperienciaViaje
        builder.Entity<ExperienciaViaje>(b =>
        {
            b.ToTable("ExperienciasViaje");
            b.ConfigureByConvention();
            b.Property(x => x.Titulo).IsRequired().HasMaxLength(128);
            b.Property(x => x.Descripcion).HasMaxLength(2000);
            // Relacion con Destino  
            b.HasOne(x => x.Destino)
             .WithMany()
             .HasForeignKey(x => x.DestinoId)
             .IsRequired();
            b.HasIndex(x => x.DestinoId);
        });

        //Aplica filtro global a todas las entidades IUserOwned EXCEPTO DestinationRating y DestinationFavorite
        // Estos manejan la seguridad a nivel de aplicación

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(IUserOwned).IsAssignableFrom(entityType.ClrType))
            {
                // NO aplicar filtro a DestinationRating ni DestinationFavorite
                // porque necesitan ser accesibles globalmente para promedios y listas públicas
                if (entityType.ClrType == typeof(DestinationRating) || 
                    entityType.ClrType == typeof(DestinationFavorite))
                {
                    continue;
                }

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

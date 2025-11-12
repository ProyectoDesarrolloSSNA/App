using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Users;
using NSubstitute;

namespace TravelBuddy.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class TravelBuddyDbContextFactory : IDesignTimeDbContextFactory<TravelBuddyDbContext>
{
    public TravelBuddyDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        TravelBuddyEfCoreEntityExtensionMappings.Configure();

        var optionsBuilder = new DbContextOptionsBuilder<TravelBuddyDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));


        // currentUser se omite: el ctor del DbContext lo acepta como null
        return new TravelBuddyDbContext(optionsBuilder.Options);

    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../TravelBuddy.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}

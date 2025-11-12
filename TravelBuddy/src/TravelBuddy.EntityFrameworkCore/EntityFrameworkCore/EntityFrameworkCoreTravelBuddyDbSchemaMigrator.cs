using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TravelBuddy.Data;
using Volo.Abp.DependencyInjection;

namespace TravelBuddy.EntityFrameworkCore;

public class EntityFrameworkCoreTravelBuddyDbSchemaMigrator
    : ITravelBuddyDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreTravelBuddyDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the TravelBuddyDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<TravelBuddyDbContext>()
            .Database
            .MigrateAsync();
    }
}

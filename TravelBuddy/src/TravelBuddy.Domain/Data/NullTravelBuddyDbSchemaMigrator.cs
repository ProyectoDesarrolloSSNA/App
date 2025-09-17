using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace TravelBuddy.Data;

/* This is used if database provider does't define
 * ITravelBuddyDbSchemaMigrator implementation.
 */
public class NullTravelBuddyDbSchemaMigrator : ITravelBuddyDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}

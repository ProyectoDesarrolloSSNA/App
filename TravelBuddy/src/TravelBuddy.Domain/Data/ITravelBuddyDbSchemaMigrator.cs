using System.Threading.Tasks;

namespace TravelBuddy.Data;

public interface ITravelBuddyDbSchemaMigrator
{
    Task MigrateAsync();
}

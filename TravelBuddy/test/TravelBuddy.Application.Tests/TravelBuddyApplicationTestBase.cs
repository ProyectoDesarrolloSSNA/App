using Volo.Abp.Modularity;

namespace TravelBuddy;

public abstract class TravelBuddyApplicationTestBase<TStartupModule> : TravelBuddyTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

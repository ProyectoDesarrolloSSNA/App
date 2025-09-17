using Volo.Abp.Modularity;

namespace TravelBuddy;

/* Inherit from this class for your domain layer tests. */
public abstract class TravelBuddyDomainTestBase<TStartupModule> : TravelBuddyTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

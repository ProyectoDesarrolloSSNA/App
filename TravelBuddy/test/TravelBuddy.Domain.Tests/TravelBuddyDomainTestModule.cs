using Volo.Abp.Modularity;

namespace TravelBuddy;

[DependsOn(
    typeof(TravelBuddyDomainModule),
    typeof(TravelBuddyTestBaseModule)
)]
public class TravelBuddyDomainTestModule : AbpModule
{

}

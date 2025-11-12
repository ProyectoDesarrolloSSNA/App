using TravelBuddy.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace TravelBuddy.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(TravelBuddyEntityFrameworkCoreModule),
    typeof(TravelBuddyApplicationContractsModule),
    typeof(TravelBuddyDomainModule)
)]
public class TravelBuddyDbMigratorModule : AbpModule
{
}

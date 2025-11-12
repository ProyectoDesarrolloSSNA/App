using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
//using TravelBuddy.Destinos;
using TravelBuddy.Destinos;
using TravelBuddy.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace TravelBuddy;

[DependsOn(
    typeof(TravelBuddyApplicationModule),
    typeof(TravelBuddyDomainTestModule),
    typeof(TravelBuddyEntityFrameworkCoreModule),
    typeof(AbpTestBaseModule)
)]
public class TravelBuddyApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Mock para ICitySearchService para evitar dependencias reales en tests
        var citySearchServiceMock = Substitute.For<ICitySearchService>();
        context.Services.AddSingleton(citySearchServiceMock);
    }
}
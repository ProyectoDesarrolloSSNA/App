using System.Net.Http;                          
using Microsoft.Extensions.DependencyInjection;   
using TravelBuddy.Destinos;
// using TravelBuddy.Application.Destinos;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace TravelBuddy;

[DependsOn(
    typeof(TravelBuddyDomainModule),
    typeof(TravelBuddyApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class TravelBuddyApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Configuración de AutoMapper para la capa de aplicación
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<TravelBuddyApplicationModule>();
        });

        // Registro del servicio HTTP que conecta con la API externa de GeoDB Cities
        context.Services.AddHttpClient<ICitySearchService, GeoDbCitySearchService>();
    }
}

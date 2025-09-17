using Microsoft.Extensions.Localization;
using TravelBuddy.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace TravelBuddy;

[Dependency(ReplaceServices = true)]
public class TravelBuddyBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<TravelBuddyResource> _localizer;

    public TravelBuddyBrandingProvider(IStringLocalizer<TravelBuddyResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}

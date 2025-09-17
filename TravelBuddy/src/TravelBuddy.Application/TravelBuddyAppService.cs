using TravelBuddy.Localization;
using Volo.Abp.Application.Services;

namespace TravelBuddy;

/* Inherit your application services from this class.
 */
public abstract class TravelBuddyAppService : ApplicationService
{
    protected TravelBuddyAppService()
    {
        LocalizationResource = typeof(TravelBuddyResource);
    }
}

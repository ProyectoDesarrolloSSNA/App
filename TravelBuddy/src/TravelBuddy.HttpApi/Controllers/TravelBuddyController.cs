using TravelBuddy.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace TravelBuddy.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class TravelBuddyController : AbpControllerBase
{
    protected TravelBuddyController()
    {
        LocalizationResource = typeof(TravelBuddyResource);
    }
}

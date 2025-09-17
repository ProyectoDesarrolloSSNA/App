using TravelBuddy.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace TravelBuddy.Permissions;

public class TravelBuddyPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(TravelBuddyPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(TravelBuddyPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<TravelBuddyResource>(name);
    }
}

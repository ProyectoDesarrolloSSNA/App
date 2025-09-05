using Volo.Abp.Settings;

namespace TravelBuddy.Settings;

public class TravelBuddySettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(TravelBuddySettings.MySetting1));
    }
}

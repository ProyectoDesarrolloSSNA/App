using TravelBuddy.Samples;
using Xunit;

namespace TravelBuddy.EntityFrameworkCore.Applications;

[Collection(TravelBuddyTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<TravelBuddyEntityFrameworkCoreTestModule>
{

}

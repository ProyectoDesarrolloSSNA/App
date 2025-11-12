using Xunit;

namespace TravelBuddy.EntityFrameworkCore;

[CollectionDefinition(TravelBuddyTestConsts.CollectionDefinitionName)]
public class TravelBuddyEntityFrameworkCoreCollection : ICollectionFixture<TravelBuddyEntityFrameworkCoreFixture>
{

}

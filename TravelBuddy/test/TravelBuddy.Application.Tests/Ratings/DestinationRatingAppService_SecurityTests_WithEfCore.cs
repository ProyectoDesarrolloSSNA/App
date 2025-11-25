using Volo.Abp.Modularity;
using Xunit;

namespace TravelBuddy.Tests.Ratings
{
    [Collection(TravelBuddyTestConsts.CollectionDefinitionName)]
    public class DestinationRatingAppService_SecurityTests_WithEfCore 
        : DestinationRatingAppService_SecurityTests<TravelBuddyApplicationTestModule>
    {
    }
}
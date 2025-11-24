using Volo.Abp.Modularity;
using Xunit;

namespace TravelBuddy.Tests.Ratings
{
    [Collection(TravelBuddyTestConsts.CollectionDefinitionName)]
    public class DestinationRatingAppService_IntegrationTests_WithEfCore 
        : DestinationRatingAppService_IntegrationTests<TravelBuddyApplicationTestModule>
    {
    }
}
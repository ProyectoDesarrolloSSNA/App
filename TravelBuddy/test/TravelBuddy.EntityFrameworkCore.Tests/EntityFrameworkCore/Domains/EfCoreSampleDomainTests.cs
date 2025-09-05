using TravelBuddy.Samples;
using Xunit;

namespace TravelBuddy.EntityFrameworkCore.Domains;

[Collection(TravelBuddyTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<TravelBuddyEntityFrameworkCoreTestModule>
{

}

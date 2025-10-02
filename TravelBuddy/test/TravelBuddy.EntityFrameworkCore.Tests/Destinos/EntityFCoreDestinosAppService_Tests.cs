using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBuddy.EntityFrameworkCore;
using Xunit;

namespace TravelBuddy.Destinos
{
    [Collection(TravelBuddyTestConsts.CollectionDefinitionName)]

    public class EntityFCoreDestinosAppService_Test : DestinosAppService_Tests<TravelBuddyEntityFrameworkCoreTestModule>
    { 
    }
}

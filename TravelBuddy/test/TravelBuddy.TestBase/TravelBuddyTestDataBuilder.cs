using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace TravelBuddy;

public class TravelBuddyTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly ICurrentTenant _currentTenant;
    private readonly IIdentityUserRepository _userRepository;
    private readonly IdentityUserManager _userManager;

    public TravelBuddyTestDataSeedContributor(
        ICurrentTenant currentTenant,
        IIdentityUserRepository userRepository,
        IdentityUserManager userManager)
    {
        _currentTenant = currentTenant;
        _userRepository = userRepository;
        _userManager = userManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        /* Seed additional test data... */

        using (_currentTenant.Change(context?.TenantId))
        {
            await SeedTestUsersAsync();
        }
    }

    private async Task SeedTestUsersAsync()
    {
        // Create test user 1 (commonly used in tests as admin user)
        var user1Id = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
        var existingUser1 = await _userRepository.FindAsync(user1Id);
        if (existingUser1 == null)
        {
            var user1 = new IdentityUser(
                user1Id,
                "test_user_" + user1Id,
                "test_user_" + user1Id + "@test.com",
                tenantId: _currentTenant.Id
            );
            await _userManager.CreateAsync(user1, "Test1234!");
        }

        // Create test user 2 (used in some tests for multi-user scenarios)
        var user2Id = Guid.Parse("3e701e62-0953-4dd3-910b-dc6cc93ccb0d");
        var existingUser2 = await _userRepository.FindAsync(user2Id);
        if (existingUser2 == null)
        {
            var user2 = new IdentityUser(
                user2Id,
                "test_user_" + user2Id,
                "test_user_" + user2Id + "@test.com",
                tenantId: _currentTenant.Id
            );
            await _userManager.CreateAsync(user2, "Test1234!");
        }
    }
}

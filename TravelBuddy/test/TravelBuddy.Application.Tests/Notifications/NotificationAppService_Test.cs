using System.Threading.Tasks;
using Shouldly;
using Xunit;
using TravelBuddy; // 1. IMPORTANTE: Necesario para encontrar TravelBuddyApplicationTestModule

namespace TravelBuddy.Notifications
{
    // 2. CORRECCIÓN AQUÍ: Agregamos <TravelBuddyApplicationTestModule>
    // Esto soluciona el error CS0305
    public class NotificationAppService_Tests : TravelBuddyApplicationTestBase<TravelBuddyApplicationTestModule>
    {
        private readonly NotificationAppService _notificationAppService;

        public NotificationAppService_Tests()
        {
            // 3. Al arreglar la línea de arriba, este método ya será reconocido
            // Esto soluciona el error CS0103
            _notificationAppService = GetRequiredService<NotificationAppService>();
        }

        [Fact]
        public async Task Should_Get_My_Notifications()
        {
            // Act
            var notifications = await _notificationAppService.GetMyNotificationsAsync();

            // Assert
            notifications.ShouldNotBeNull();
        }
    }
}

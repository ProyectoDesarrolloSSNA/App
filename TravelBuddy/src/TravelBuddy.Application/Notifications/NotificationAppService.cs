using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace TravelBuddy.Notifications
{
    [Authorize]
    public class NotificationAppService : TravelBuddyAppService, IApplicationService
    {
        private readonly IRepository<AppNotification, Guid> _notificationRepository;

        public NotificationAppService(IRepository<AppNotification, Guid> notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<List<AppNotificationDto>> GetMyNotificationsAsync()
        {
            var userId = CurrentUser.GetId();

            // Obtenemos las notificaciones del usuario actual, ordenadas por fecha
            var notifications = await _notificationRepository.GetListAsync(x => x.UserId == userId);

            return ObjectMapper.Map<List<AppNotification>, List<AppNotificationDto>>(
                notifications.OrderByDescending(x => x.CreationTime).ToList()
            );
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            var notification = await _notificationRepository.GetAsync(id);

            // Seguridad: verificar que la notificación pertenezca al usuario
            if (notification.UserId != CurrentUser.GetId())
            {
                throw new UnauthorizedAccessException("No puedes modificar esta notificación.");
            }

            notification.MarkAsRead();
            await _notificationRepository.UpdateAsync(notification);
        }
    }
}
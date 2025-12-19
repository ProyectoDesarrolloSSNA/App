using System;
using Volo.Abp.Application.Dtos;

namespace TravelBuddy.Notifications
{
    public class AppNotificationDto : EntityDto<Guid>
    {
        public required string Title { get; set; }
        public required string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
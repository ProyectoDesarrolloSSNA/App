using System;
using Volo.Abp.Application.Dtos;

namespace TravelBuddy.Notifications
{
    public class AppNotificationDto : EntityDto<Guid>
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
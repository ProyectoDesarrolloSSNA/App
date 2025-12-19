using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace TravelBuddy.Notifications
{
    public class AppNotification : CreationAuditedEntity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public string? RelatedEntityId { get; set; } // Para guardar el ID del Destino
        public string? RelatedEntityType { get; set; } // Ejemplo: "Destino"

        protected AppNotification() { }

        public AppNotification(Guid id, Guid userId, string title, string message, Guid? tenantId = null)
            : base(id)
        {
            UserId = userId;
            Title = title;
            Message = message;
            TenantId = tenantId;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }

        public void MarkAsUnread()
        {
            IsRead = false;
        }
    }
}

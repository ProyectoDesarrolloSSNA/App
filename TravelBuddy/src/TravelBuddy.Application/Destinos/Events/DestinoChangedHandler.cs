using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;
using Volo.Abp.Identity;
using TravelBuddy.Notifications;

namespace TravelBuddy.Destinos.Events
{
    public class DestinoChangedHandler : ILocalEventHandler<DestinoChangedEto>, ITransientDependency
    {
        private readonly IRepository<AppNotification, Guid> _notificationRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository; // Para obtener a quién notificar

        public DestinoChangedHandler(
            IRepository<AppNotification, Guid> notificationRepository,
            IRepository<IdentityUser, Guid> userRepository)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
        }

        public async Task HandleEventAsync(DestinoChangedEto eventData)
        {
            // LOGICA DE NEGOCIO: ¿A quién notificamos?
            // Por simplicidad para la tarea 7.2, notificaremos a todos los usuarios excepto al Admin (o todos).
            // En un escenario real, filtrarías por "Usuarios que siguen este destino".

            var users = await _userRepository.GetListAsync();

            foreach (var user in users)
            {
                // Creamos la notificación persistente
                var notification = new AppNotification(
                    Guid.NewGuid(),
                    user.Id,
                    title: $"Novedades en {eventData.DestinoNombre}",
                    message: $"El destino ha sido marcado como: {eventData.TipoCambio}. ¡Reísaloooo!"
                );

                notification.RelatedEntityId = eventData.DestinoId.ToString();
                notification.RelatedEntityType = "Destino";

                await _notificationRepository.InsertAsync(notification);
            }
        }
    }
}

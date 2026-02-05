using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TravelBuddy.Users
{
    public interface IPublicProfileAppService : IApplicationService
    {
        /// <summary>
        /// Obtiene el perfil público de un usuario por su ID
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Perfil público del usuario</returns>
        Task<PublicProfileDto> GetPublicProfileAsync(Guid userId);
        
        /// <summary>
        /// Obtiene el perfil público de un usuario por su nombre de usuario
        /// </summary>
        /// <param name="userName">Nombre de usuario</param>
        /// <returns>Perfil público del usuario</returns>
        Task<PublicProfileDto> GetPublicProfileByUserNameAsync(string userName);
    }
}

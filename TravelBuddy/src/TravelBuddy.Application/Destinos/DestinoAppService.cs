using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TravelBuddy.Destinos
{
    public class DestinoAppService : ApplicationService
    {
        private readonly IRepository<Destino, Guid> _repositorio;

        public DestinoAppService(IRepository<Destino, Guid> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<DestinoDto> CrearAsync(CreateDestinoDto input)
        {
            if (string.IsNullOrWhiteSpace(input.Nombre))
                throw new ArgumentException("El nombre es obligatorio.");

            var destino = new Destino(
                GuidGenerator.Create(),
                input.Nombre,
                input.Pais,
                input.Descripcion
            );

            await _repositorio.InsertAsync(destino, autoSave: true);

            return ObjectMapper.Map<Destino, DestinoDto>(destino);
        }
    }
}
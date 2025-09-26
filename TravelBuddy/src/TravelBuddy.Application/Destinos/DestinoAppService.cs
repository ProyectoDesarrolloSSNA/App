using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TravelBuddy.Destinos
{
    public class DestinoAppService :
    CrudAppService<
        Destino,
        DestinoDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateDestinoDto>,
    IDestinoAppService
    {
        public DestinoAppService(IRepository<Destino, Guid> repo) : base(repo)
        {
        }
    }
}
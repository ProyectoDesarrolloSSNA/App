using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace TravelBuddy.Destinos
{
    public interface IDestinoAppService :
        ICrudAppService<
            DestinoDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateDestinoDto> 
    {
    }
}

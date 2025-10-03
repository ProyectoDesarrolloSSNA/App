using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelBuddy.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Volo.Abp.Validation;
using Xunit;

namespace TravelBuddy.Destinos
{
    public abstract class DestinosAppService_Tests<TStartupModule> : TravelBuddyApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IDestinoAppService _service;
        private readonly IDbContextProvider<TravelBuddyDbContext> _DbContextProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;


        protected DestinosAppService_Tests()
        {
            _service = GetRequiredService<IDestinoAppService>();
            _DbContextProvider = GetRequiredService<IDbContextProvider<TravelBuddyDbContext>>();
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
        }

        [Fact]
        public async Task ShouldReturnCreatedDestinationDto()
        {
            //Arrange 
            var input = new CreateUpdateDestinoDto
            {
                Nombre = "París",
                Pais = "Francia",
                Descripcion = "La ciudad del amor y la luz."
            };

            //Act
            var result = await _service.CreateAsync(input);

            //Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
            result.Nombre.ShouldBe(input.Nombre);
            result.Pais.ShouldBe(input.Pais);
            result.Descripcion.ShouldNotBeEmpty();
        }
        [Fact]
        public async Task CreateAsync_ShouldPersistDestinoInDatabase()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                //Arrange
                var input = new CreateUpdateDestinoDto
                {
                    Nombre = "Tokyo",
                    Pais = "Japon",
                    Descripcion = "..."
                };

                //Act
                var result = await _service.CreateAsync(input);


                //Assert
                var dbContext = await _DbContextProvider.GetDbContextAsync();
                var savedDestino = await dbContext.Destinos.FindAsync(result.Id);

                savedDestino.ShouldNotBeNull();
                savedDestino.Nombre.ShouldBe(input.Nombre);
                savedDestino.Pais.ShouldBe(input.Pais);
                savedDestino.Descripcion.ShouldBe(input.Descripcion);
                

            }
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowExceptionWhenCountryIsNull()
        {
            //Arreange
            var input = new CreateUpdateDestinoDto
            {
                Nombre = "",
                Pais = null,
                Descripcion = ""
            };

            //Act ^ Assert
            await Should.ThrowAsync<AbpValidationException>(async () => { await _service.CreateAsync(input); });
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowExceptionWhenNameIsNull()
        {
            //Arreange
            var input = new CreateUpdateDestinoDto
            {
                Nombre = null,
                Pais = "",
                Descripcion = ""
            };

            //Act ^ Assert
            await Should.ThrowAsync<AbpValidationException>(async () => { await _service.CreateAsync(input); });
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowExceptionWhenDescriptionIsNull()
        {
            //Arreange
            var input = new CreateUpdateDestinoDto
            {
                Nombre = "",
                Pais = "",
                Descripcion = null
            };

            //Act ^ Assert
            await Should.ThrowAsync<AbpValidationException>(async () => { await _service.CreateAsync(input); });
        }

    }

}

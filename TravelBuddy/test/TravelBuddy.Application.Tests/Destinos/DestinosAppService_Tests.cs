using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelBuddy.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Volo.Abp.Validation;
using Volo.Abp.EventBus.Local;
using Xunit;

// Agregados para mocks
using NSubstitute;
using Volo.Abp.Domain.Repositories;

// Tipos de tu dominio/aplicación
using TravelBuddy.Destinos;

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

        // ------------------------
        // Tests CRUD existentes
        // ------------------------

        [Fact]
        public async Task ShouldReturnCreatedDestinationDto()
        {
            // Arrange 
            var input = new CreateUpdateDestinoDto
            {
                Nombre = "París",
                Pais = "Francia",
                Descripcion = "La ciudad del amor y la luz."
            };

            // Act
            var result = await _service.CreateAsync(input);

            // Assert
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
                // Arrange
                var input = new CreateUpdateDestinoDto
                {
                    Nombre = "Tokyo",
                    Pais = "Japon",
                    Descripcion = "..."
                };

                // Act
                var result = await _service.CreateAsync(input);

                // Assert
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
            // Arrange
            var input = new CreateUpdateDestinoDto
            {
                Nombre = "",
                Pais = null,
                Descripcion = ""
            };

            // Act & Assert
            await Should.ThrowAsync<AbpValidationException>(async () => { await _service.CreateAsync(input); });
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowExceptionWhenNameIsNull()
        {
            // Arrange
            var input = new CreateUpdateDestinoDto
            {
                Nombre = null,
                Pais = "",
                Descripcion = ""
            };

            // Act & Assert
            await Should.ThrowAsync<AbpValidationException>(async () => { await _service.CreateAsync(input); });
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowExceptionWhenDescriptionIsNull()
        {
            // Arrange
            var input = new CreateUpdateDestinoDto
            {
                Nombre = "",
                Pais = "",
                Descripcion = null
            };

            // Act & Assert
            await Should.ThrowAsync<AbpValidationException>(async () => { await _service.CreateAsync(input); });
        }

        // --------------------------------------------------------
        // NUEVOS tests unitarios para la búsqueda externa de ciudades
        // Mockeando ICitySearchService y el repositorio de Destino
        // --------------------------------------------------------

        private static DestinoAppService CreateAppServiceWithMocks(
            out IRepository<Destino, Guid> repoMock,
            out ICitySearchService citySearchMock)
        {
            repoMock = Substitute.For<IRepository<Destino, Guid>>();
            citySearchMock = Substitute.For<ICitySearchService>();

            // CREAMOS EL MOCK DEL EVENT BUS
            var localEventBusMock = Substitute.For<ILocalEventBus>();

            return new DestinoAppService(repoMock, citySearchMock,localEventBusMock);
        }

        [Fact]
        public async Task BuscarPorNombreExternamenteAsync_RetornaResultados()
        {
            // Arrange
            var nombre = "Test";
            var expected = new List<CityDto>
            {
                new CityDto { Name = "TestCity", Country = "TestCountry", Latitude = -34.6, Longitude = -58.4 }
            };

            var service = CreateAppServiceWithMocks(out var repoMock, out var citySearchMock);

            citySearchMock
                .BuscarCiudadesPorNombreAsync(nombre)
                .Returns(expected);

            // Act
            var result = await service.BuscarPorNombreExternamenteAsync(nombre);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].Name.ShouldBe("TestCity");
            result[0].Country.ShouldBe("TestCountry");
        }

        [Fact]
        public async Task BuscarPorNombreExternamenteAsync_SinCoincidencias_RetornaVacio()
        {
            // Arrange
            var nombre = "NoMatch";
            var expected = new List<CityDto>();

            var service = CreateAppServiceWithMocks(out var repoMock, out var citySearchMock);

            citySearchMock
                .BuscarCiudadesPorNombreAsync(nombre)
                .Returns(expected);

            // Act
            var result = await service.BuscarPorNombreExternamenteAsync(nombre);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task BuscarPorNombreExternamenteAsync_InputInvalido_RetornaVacio()
        {
            // Arrange
            var nombre = ""; // si en el futuro cambias la lógica para no llamar al servicio con input inválido, ajusta este test
            var expected = new List<CityDto>();

            var service = CreateAppServiceWithMocks(out var repoMock, out var citySearchMock);

            citySearchMock
                .BuscarCiudadesPorNombreAsync(nombre)
                .Returns(expected);

            // Act
            var result = await service.BuscarPorNombreExternamenteAsync(nombre);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task BuscarPorNombreExternamenteAsync_ErrorApi_PropagaExcepcion()
        {
            // Arrange
            var nombre = "Test";
            var service = CreateAppServiceWithMocks(out var repoMock, out var citySearchMock);

            citySearchMock
                .When(x => x.BuscarCiudadesPorNombreAsync(nombre))
                .Do(_ => throw new Exception("API error"));

            // Act & Assert
            await Should.ThrowAsync<Exception>(() => service.BuscarPorNombreExternamenteAsync(nombre));
        }
    }
}
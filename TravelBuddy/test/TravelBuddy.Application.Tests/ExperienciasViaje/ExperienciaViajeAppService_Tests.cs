using Shouldly;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TravelBuddy.Destinos; 
using TravelBuddy.ExperienciasViaje;
using TravelBuddy.ExperienciasViaje.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Security.Claims;
using Volo.Abp.Validation;
using Xunit;

namespace TravelBuddy.Application.Tests.ExperienciasViaje
{
    public class ExperienciaViajeAppService_Tests : TravelBuddyApplicationTestBase<TravelBuddyApplicationTestModule>
    {
        private readonly IExperienciaViajeAppService _experienciaAppService;
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;
        private readonly IRepository<Destino, Guid> _destinoRepository; 

        public ExperienciaViajeAppService_Tests()
        {
            _experienciaAppService = GetRequiredService<IExperienciaViajeAppService>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
            _destinoRepository = GetRequiredService<IRepository<Destino, Guid>>();
        }

        [Fact]
        public async Task CreateAsync_Should_Create_Experience_With_Current_User()
            {
            // Arrange
            var testUserId = Guid.NewGuid(); 

            // 1. Primero creamos un Destino real en la BD (Requisito de FK)
            var destino = await _destinoRepository.InsertAsync(new Destino(
                Guid.NewGuid(),
                "Barcelona",
                "España",
                "..."
            ));

            await WithUnitOfWorkAsync(async () =>
            {
                // 2. Simulamos el usuario autenticado
                var claims = new[]
                {
                    new Claim(AbpClaimTypes.UserId, testUserId.ToString()),
                    new Claim(AbpClaimTypes.UserName, "viajero_pro")
                };

                var identity = new ClaimsIdentity(claims, "TestAuthType");
                var principal = new ClaimsPrincipal(identity);

                using (_currentPrincipalAccessor.Change(principal))
                {
                    var input = new CrearActualizarExperienciaViajeDto
                    {
                        DestinoId = destino.Id,
                        Titulo = "Prueba0",
                        Descripcion = "Pruba1",
                        Calificacion = 3
                    };

                    // Act
                    var result = await _experienciaAppService.CreateAsync(input);

                    // Assert
                    result.ShouldNotBeNull();
                    result.Titulo.ShouldBe("Prueba0");
                    result.CreatorId.ShouldBe(testUserId); // Verificamos que se guardó con el usuario simulado
                }
            });
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_If_Not_Authenticated()
        {
            // Arrange
            var input = new CrearActualizarExperienciaViajeDto
            {
                DestinoId = Guid.NewGuid(),
                Titulo = "Intento",
                Descripcion = "Sin login",
                Calificacion = 5
            };

            // Act & Assert
            // Esperamos AbpAuthorizationException porque el servicio tiene [Authorize]
            await Should.ThrowAsync<AbpAuthorizationException>(async () =>
            {
                await _experienciaAppService.CreateAsync(input);
            });
        }

        [Fact]
        public async Task GetListAsync_Should_Filter_By_Text()
        {
            // Arrange
            // Creamos destino y datos semilla
            var destino = await _destinoRepository.InsertAsync(new Destino
                (Guid.NewGuid(), "Roma", "Italia", "Historia"));

            // Simulamos usuario para poder crear (si no, fallaría el Authorize al crear los datos de prueba)
            var claims = new[] { new Claim(AbpClaimTypes.UserId, Guid.NewGuid().ToString()) };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthType"));

            using (_currentPrincipalAccessor.Change(principal))
            {
                await _experienciaAppService.CreateAsync(new CrearActualizarExperienciaViajeDto
                {
                    DestinoId = destino.Id,
                    Titulo = "Visita al Coliseo",
                    Descripcion = "Increíble historia",
                    Calificacion = 5
                });

                await _experienciaAppService.CreateAsync(new CrearActualizarExperienciaViajeDto
                {
                    DestinoId = destino.Id,
                    Titulo = "Comida",
                    Descripcion = "La mejor Comida", // Palabra clave para buscar
                    Calificacion = 5
                });
            }

            // Act: Filtramos buscando "carbonara"
            var input = new GetExperienciasViajeInput { TextoFiltro = "Comida" };

            // Nota: GetListAsync también requiere autenticación según tu AppService
            using (_currentPrincipalAccessor.Change(principal))
            {
                var result = await _experienciaAppService.GetListAsync(input);

                // Assert
                result.TotalCount.ShouldBe(1);
                result.Items.First().Titulo.ShouldBe("Comida");
            }
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_Validation_Error_If_Rating_Invalid()
        {
            // Arrange
            var destino = await _destinoRepository.InsertAsync(new Destino
                (Guid.NewGuid(), "Paris", "Francia", " Amor"));
            var userId = Guid.NewGuid();
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(AbpClaimTypes.UserId, userId.ToString()) }, "TestAuthType"));

            using (_currentPrincipalAccessor.Change(principal))
            {
                var input = new CrearActualizarExperienciaViajeDto
                {
                    DestinoId = destino.Id,
                    Titulo = "Rating Malo",
                    Calificacion = 10 // INVÁLIDO (El DTO permite máx 5)
                };

                // Act & Assert
                // AbpValidationException salta automáticamente por los DataAnnotations [Range(1,5)] del DTO
                await Should.ThrowAsync<AbpValidationException>(async () =>
                {
                    await _experienciaAppService.CreateAsync(input);
                });
            }
        }
    }
}
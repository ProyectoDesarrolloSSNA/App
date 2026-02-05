using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenIddict.Abstractions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace TravelBuddy.DbMigrator
{
    public class OpenIddictDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IOpenIddictApplicationManager _applicationManager;

        public OpenIddictDataSeedContributor(IOpenIddictApplicationManager applicationManager)
        {
            _applicationManager = applicationManager;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            await CreateSwaggerClientAsync();
        }

        private async Task CreateSwaggerClientAsync()
        {
            const string clientName = "TravelBuddy_Swagger";

            var application = await _applicationManager.FindByClientIdAsync(clientName);

            if (application == null)
            {
                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = clientName,
                    ClientSecret = "dev-secret-123!",
                    DisplayName = "Swagger Client",
                    // 👇 CORREGIDO AQUÍ: Se llama ClientType, no Type
                    ClientType = OpenIddictConstants.ClientTypes.Confidential,
                    ConsentType = OpenIddictConstants.ConsentTypes.Implicit,

                    RedirectUris = { new Uri("https://localhost:44367/swagger/oauth2-redirect.html") },

                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,

                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                        OpenIddictConstants.Permissions.GrantTypes.Password, // IMPORTANTE
                        
                        OpenIddictConstants.Permissions.ResponseTypes.Code,

                        OpenIddictConstants.Permissions.Prefixes.Scope + "TravelBuddy",
                        OpenIddictConstants.Permissions.Prefixes.Scope + "openid",
                        OpenIddictConstants.Permissions.Prefixes.Scope + "profile",
                        OpenIddictConstants.Permissions.Prefixes.Scope + "email",
                        OpenIddictConstants.Permissions.Prefixes.Scope + "roles",
                        OpenIddictConstants.Permissions.Prefixes.Scope + "offline_access"
                    }
                };

                await _applicationManager.CreateAsync(descriptor);
            }
            else
            {
                var permissions = await _applicationManager.GetPermissionsAsync(application);
                if (!permissions.Contains(OpenIddictConstants.Permissions.GrantTypes.Password))
                {
                    var descriptor = new OpenIddictApplicationDescriptor();
                    await _applicationManager.PopulateAsync(descriptor, application);

                    descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Password);

                    await _applicationManager.UpdateAsync(application, descriptor);
                }
            }
        }
    }
}
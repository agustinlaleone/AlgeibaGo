using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace IdentityServerAspNetIdentity
{
    public class Config
    {
        private IConfiguration Configuration { get; }
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.Email(),
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("AlgeibaGoManagerDBApi", "My API")
            };
        }

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            string webURL = configuration.GetSection("AlgGoURL").Value;
            string callbackURL = configuration.GetSection("RedirectUris").Value;
            string clientId = configuration.GetSection("LocalClientId").Value;
            return new List<Client>
            {
                new Client
                {
                    ClientId = clientId,
                    ClientName = "AlgeibaGo",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireClientSecret = false,
                    RequireConsent = false,
                    AllowAccessTokensViaBrowser = true,
                    
                    RedirectUris =           { callbackURL },
                    PostLogoutRedirectUris = { webURL },
                    AllowedCorsOrigins =     { webURL },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Profile,
                        "AlgeibaGoManagerDBApi"
                    }
                }
            };
        }
    }
}
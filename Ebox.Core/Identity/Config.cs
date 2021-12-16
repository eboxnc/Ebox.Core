using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;


namespace Ebox.Core
{
    public class Config
    {
        public const string ClientId = "9e281029-bb03-4cbb-925f-e844589f200b";
        public const string ClientSecret = "53267724-c26a-4f15-9c0b-cb61c91c4c90";
        public const string Scope = "EboxApi_scope";

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource(Scope, "Ebox API Scope")
                {
                    Scopes = { Scope }
                }
            };
        }

        public static IEnumerable<ApiScope> GetScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope(Scope, "Ebox API Scope")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = ClientId,
                    ClientName = "api1-client",
                    AllowedGrantTypes =GrantTypes.ResourceOwnerPassword,
                    AccessTokenLifetime = 3600 * 24 * 7, //7天
                    AllowOfflineAccess = true,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    SlidingRefreshTokenLifetime = 3600 * 24 * 5, //5天
                    ClientSecrets = {new Secret(ClientSecret.ToSha256())},
                    AllowedScopes = {
                        Scope,
                        "offline_access"
                    }
                }
            };
        }
    }
}

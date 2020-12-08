using Tickr.Client.Configurations;

namespace Tickr.Client.Helpers
{
    using System.Threading.Tasks;
    using Auth0.AuthenticationApi;
    using Auth0.AuthenticationApi.Models;
    using Microsoft.Extensions.Configuration;


    public class AuthorizationHelper : IAuthorizationHelper
    {
        private readonly IConfiguration _configuration;
        private readonly AuthSettings _authSettings;

        public AuthorizationHelper(IConfiguration configuration, AuthSettings authSettings)
        {
            _configuration = configuration;
            _authSettings = authSettings;
        }
        public async Task<string> GetAccessToken()
        {
            var auth0Client = new AuthenticationApiClient(_authSettings.Domain);
            var tokenRequest = new ClientCredentialsTokenRequest()
            {
                ClientId = _authSettings.ClientId,
                ClientSecret = _authSettings.ClientSecret,
                Audience = _authSettings.Audience
            };
            var tokenResponse = await auth0Client.GetTokenAsync(tokenRequest);

            return tokenResponse.AccessToken;
        }
    }
}

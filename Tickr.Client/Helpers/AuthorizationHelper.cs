namespace Tickr.Client.Helpers
{
    using System.Threading.Tasks;
    using Auth0.AuthenticationApi;
    using Auth0.AuthenticationApi.Models;
    using Microsoft.Extensions.Configuration;


    public class AuthorizationHelper : IAuthorizationHelper
    {
        private readonly IConfiguration _configuration;

        public AuthorizationHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> GetAccessToken()
        {
            var appAuth0Settings = _configuration.GetSection("Auth0");
            var auth0Client = new AuthenticationApiClient(appAuth0Settings["Domain"]);
            var tokenRequest = new ClientCredentialsTokenRequest()
            {
                ClientId = appAuth0Settings["ClientId"],
                ClientSecret = appAuth0Settings["ClientSecret"],
                Audience = appAuth0Settings["Audience"]
            };
            var tokenResponse = await auth0Client.GetTokenAsync(tokenRequest);

            return tokenResponse.AccessToken;
        }
    }
}

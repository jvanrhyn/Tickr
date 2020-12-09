namespace Tickr.Client.Helpers
{
    using System.Threading.Tasks;
    using Grpc.Core;

    public class AuthorizationMetadataBuilder
    {
        private readonly IAuthorizationHelper _authorizationHelper;

        public AuthorizationMetadataBuilder(IAuthorizationHelper authorizationHelper)
        {
            _authorizationHelper = authorizationHelper;
        }
        
        public async Task<Metadata> GetAuthorizationHeader()
        {
            var accessToken = await _authorizationHelper.GetAccessToken();
            var headers = new Metadata {{"Authorization", $"Bearer {accessToken}"}};
            return headers;
        }
    }
}
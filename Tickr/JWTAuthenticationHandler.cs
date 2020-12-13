namespace Tickr
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class JwtAuthenticationHandler : JwtBearerHandler
    {
        public JwtAuthenticationHandler(IOptionsMonitor<JwtBearerOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorityConfig =
                await Options.ConfigurationManager.GetConfigurationAsync(Context.RequestAborted);
            var authorityIssuer = authorityConfig.Issuer;

            var jwtToken = ReadTokenFromHeader();

            var jwtHandler = new JwtSecurityTokenHandler();

            if (jwtHandler.CanReadToken(jwtToken))
            {
                var token = jwtHandler.ReadJwtToken(jwtToken);
                if (string.Equals(token.Issuer, authorityIssuer, StringComparison.OrdinalIgnoreCase))
                {
                    return await base.HandleAuthenticateAsync();
                }
                else
                {
                    Logger.LogDebug(
                        $"Skipping jwt token validation because token issuer was {token.Issuer} but the authority issuer is: {authorityIssuer}");
                    return AuthenticateResult.NoResult();
                }
            }

            return await base.HandleAuthenticateAsync();
        }

        private string ReadTokenFromHeader()
        {
            string token = null;

            string authorization = Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authorization))
            {
                return null;
            }

            if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = authorization.Substring("Bearer ".Length).Trim();
            }

            return token;
        }
    }
}

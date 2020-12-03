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

    public class JWTAuthenticationHandler : JwtBearerHandler
    {
        public JWTAuthenticationHandler(IOptionsMonitor<JwtBearerOptions> options, ILoggerFactory logger,
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
                    // means the token was issued by this authority, we make sure full validation runs as normal
                    return await base.HandleAuthenticateAsync();
                }
                else
                {
                    // Skip validation since the token as issued by a an issuer that this instance doesn't know about
                    // That has zero of success, so we will not issue a "fail" since it crowds the logs with failures of type IDX10501 
                    // which are not really true and certainly not useful.
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

            // If no authorization header found, nothing to process further
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

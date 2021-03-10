namespace Tickr.Tests.Unit
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    public class JwtAuthenticationHandlerTests
    {
        [Fact]
        public void JwtTest()
        {
            var optionsMock = new Mock<OptionsMonitor<JwtBearerOptions>>();
            var clockMock = new Mock<ISystemClock>();
            //JwtAuthenticationHandler jwtAuthenticationHandler = new(optionsMock.Object, NullLoggerFactory.Instance, new UrlTestEncoder(),clockMock.Object);
        }
    }
}
using Shouldly;
using SimpleAuthentication.Core.Config;
using SimpleAuthentication.Core.Exceptions;
using Xunit;

namespace SimpleAuthentication.Tests.Config
{
    public class AppConfigServiceFacts
    {
        [Fact]
        public void GivenNoAppSettings_RedirectToProvider_ThrowsAnException()
        {
            // Arrange.
            var configService = new AppConfigService();

            // Act.
            var exception = Should.Throw<AuthenticationException>(() => configService.GetConfiguration());

            // Assert.
            exception.Message.ShouldBe("No AppSettings keys -or- a SimpleAuthentication configuration section was found. At least one of those configration options is required - otherwise, how do we know ~how~ to connect to any Authentication Providers which you have required? A sample <appSettings> key/value is: <add key=\"sa.Google\" value=\"key:blahblahblah.apps.googleusercontent.com;secret:pew-pew\". Please check the documentation about how to add your specific configuration settings./>");
        }
    }
}

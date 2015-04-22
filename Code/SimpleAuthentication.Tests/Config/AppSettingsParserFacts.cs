using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Shouldly;
using SimpleAuthentication.Core.Config;
using Xunit;

namespace SimpleAuthentication.Tests.Config
{
    public class AppSettingsParserFacts
    {
        public class ParseAppSettingsFacts
        {
            [Fact]
            public void GivenAFileWithAnAppSettingsWithSomeProviders_ParseAppSettings_ReturnsAConfiguration()
            {
                // Arrange & Act.
                var configuration = AppSettingsParser.ParseAppSettings("Config\\SampleAppSettingsConfigSettings.xml");

                // Assert.
                configuration.Providers.Count.ShouldBe(6);

                var provider = configuration.Providers.Single(x => x.Name == "Google");
                provider.Key.ShouldBe("587140099194.apps.googleusercontent.com");
                provider.Scopes.ShouldBe("scope1,scope2,scope3");
                provider.Secret.ShouldBe("aaaaaa");

                configuration.CallBackRoute.ShouldBe("~/a/b/c?x=1&y=2");
                configuration.RedirectRoute.ShouldBe("~/x/y/z?a=1&b=2");
            }

            [Fact]
            public void GivenAFileWithAppSettingsButNoProviders_ParseAppSettings_ReturnsANullConfiguration()
            {
                // Arrange & Act.
                var configuration = AppSettingsParser.ParseAppSettings("Config\\SampleAppSettingsWithNoProvidersConfigSettings.xml");

                // Assert.
                configuration.CallBackRoute.ShouldBe(null);
                configuration.RedirectRoute.ShouldBe(null);
                configuration.Providers.ShouldBe(null);
            }

            [Fact]
            public void GivenNoSettingsFile_ParseAppSettings_ReturnsANullConfiguration()
            {
                // Arrange & Assert.
                var configuration = AppSettingsParser.ParseAppSettings();

                configuration.ShouldBe(null);
            }
        }
    }
}
using System.Collections.Specialized;
using System.Linq;
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
            public void GivenANameValueCollectionWithOneKeyAndSecret_ParseAppSettings_ReturnsAConfiguration()
            {
                // Arrange.
                const string key = "111.apps.googleusercontent.com";
                const string secret = "daskfksadhf";

                var appConfigSettings = new NameValueCollection
                {
                    {"a", "b"},
                    {"sa.Google", string.Format("key:{0};secret:{1}", key, secret)}
                };

                // Act.
                var configuration = appConfigSettings.ParseAppSettings();

                // Assert.`
                configuration.CallBackRoute.ShouldBe(null);
                configuration.RedirectRoute.ShouldBe(null);
                configuration.Providers.Count.ShouldBe(1);

                var provider = configuration.Providers.First();
                provider.Key.ShouldBe(key);
                provider.Name.ShouldBe("google");
                provider.Scopes.ShouldBe(null);
                provider.Secret.ShouldBe(secret);
            }

            [Fact]
            public void GivenAnEmptyNameValueCollection_ParseAppSettings_ReturnsNoConfiguration()
            {
                // Arrange.
                var appConfigSettings = new NameValueCollection();

                // Act.
                var configuration = appConfigSettings.ParseAppSettings();

                // Assert.
                configuration.CallBackRoute.ShouldBe(null);
                configuration.RedirectRoute.ShouldBe(null);
                configuration.Providers.ShouldBe(null);
            }

            [Fact]
            public void GivenANameValueCollectionWithACallbackRoute_ParseAppSettings_ReturnsAConfiguration()
            {
                // Arrange.
                const string callbackRoute = "afsdfds";
                var appConfigSettings = new NameValueCollection
                {
                    {"sa.callbackroute", callbackRoute}
                };

                // Act.
                var configuration = appConfigSettings.ParseAppSettings();

                // Assert.
                configuration.CallBackRoute.ShouldBe(callbackRoute);
                configuration.RedirectRoute.ShouldBe(null);
                configuration.Providers.ShouldBe(null);
            }

            [Fact]
            public void GivenANameValueCollectionWithARedirectRoute_ParseAppSettings_ReturnsAConfiguration()
            {
                // Arrange.
                const string redirectRoute = "afsdfds";
                var appConfigSettings = new NameValueCollection
                {
                    {"sa.redirectroute", redirectRoute}
                };

                // Act.
                var configuration = appConfigSettings.ParseAppSettings();

                // Assert.
                configuration.CallBackRoute.ShouldBe(null);
                configuration.RedirectRoute.ShouldBe(redirectRoute);
                configuration.Providers.ShouldBe(null);
            }
        }
    }
}
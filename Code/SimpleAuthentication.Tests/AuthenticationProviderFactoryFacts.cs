using FakeItEasy;
using Shouldly;
using SimpleAuthentication.Core;
using SimpleAuthentication.Core.Config;
using SimpleAuthentication.Core.Exceptions;
using SimpleAuthentication.Core.Providers;
using Xunit;

namespace SimpleAuthentication.Tests
{
    public class AuthenticationProviderFactoryFacts
    {

        public class AuthenticationProviderConstructorFacts
        {
            [Fact]
            public void GivenAConfigServiceWithNoProviders_Constructor_ThrowsAnException()
            {
                // Arrange.
                var configService = A.Fake<IConfigService>();
                // NOTE: no Providers are set, here. This emulates that no
                //       providers were found in the appSettings.
                A.CallTo(() => configService.GetConfiguration()).Returns(new Configuration());

                var providerScanner = A.Fake<IProviderScanner>();

                // Act.
                var exception = Should.Throw<AuthenticationException>(() =>
                    new AuthenticationProviderFactory(configService, providerScanner));

                // Assert.
                exception.Message.ShouldBe("There needs to be at least one Authentication Provider's detail's in the configService.Provider's collection. Otherwise, how else are we to set the available Authentication Providers?");
            }

            [Fact]
            public void GivenAProviderScannerWithNoDiscoveredProviders_Constructor_ThrowsAnException()
            {
                // Arrange.
                var configService = A.Fake<IConfigService>();
                A.CallTo(() => configService.GetConfiguration()).Returns(TestHelpers.ConfigurationProviders);

                // NOTE: This provider scanner defaults to returning a null Type list.
                var providerScanner = A.Fake<IProviderScanner>();

                // Act.
                var exception = Should.Throw<AuthenticationException>(() =>
                    new AuthenticationProviderFactory(configService, providerScanner));

                // Assert.
                exception.Message.ShouldBe("No discovered providers were found by the Provider Scanner. We need at least one IAuthenticationProvider type to exist so we can attempt to map the authentication data (from the configService) to the found Provider.");
            }

            [Fact]
            public void GivenAConfigProviderThanDoesntExistInADiscoveredProviders_Constructor_ThrowsAnException()
            {
                // Arrange.
                var configService = A.Fake<IConfigService>();
                A.CallTo(() => configService.GetConfiguration()).Returns(TestHelpers.ConfigurationProviders);

                // NOTE: This provider scanner defaults to returning a null Type list.
                var discoveredProviders = new[] {typeof (FacebookProvider)};
                var providerScanner = A.Fake<IProviderScanner>();
                A.CallTo(() => providerScanner.GetDiscoveredProviders()).Returns(discoveredProviders);

                // Act.
                var exception = Should.Throw<AuthenticationException>(() =>
                    new AuthenticationProviderFactory(configService, providerScanner));

                // Assert.
                exception.Message.ShouldBe("You have provided some configuration settings for an Authentication Provider: 'google' but no Authentication Provider was setup with this name. Is there a provider dll available? Is there a typo in the provider name? Possible suggestions to fix this: Make sure you have correctly setup/defined the Authentication Providers you want. If you are using the default ProviderScanner, you can manually define which providers to use. If you don't then only the default 4 are setup -> Google, Facebook, Twitter and Windows Live. For more information on this, please check the documentation/wiki on the github website and look for 'ProviderScanners'.");
            }
        }
    }
}
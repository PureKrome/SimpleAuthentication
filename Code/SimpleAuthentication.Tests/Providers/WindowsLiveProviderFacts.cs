using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Shouldly;
using SimpleAuthentication.Core;
using WorldDomination.Net.Http;
using Xunit;

namespace SimpleAuthentication.Tests.Providers
{
    public class WindowsLiveProviderFacts
    {
        public class GetRedirectToAuthenticateSettingsFacts
        {
            [Fact]
            public void GivenACallbackUrl_GetRedirectToAuthenticateSettingsFacts_ReturnsSomeRedirectToAUthenticateSettings()
            {
                // Arramge.
                var provider = TestHelpers.GetAuthenticationProvider(TestHelpers.WindowsLiveProvider.Name);

                var callbackUri = new Uri("http://www.localhost.me?provider=windowsLive");

                // Act.
                var result = provider.GetRedirectToAuthenticateSettings(callbackUri);

                // Assert.
                result.State.ShouldNotBeNullOrEmpty();
                result.RedirectUri.AbsoluteUri.ShouldBe(
                    string.Format("https://login.live.com/oauth20_authorize.srf?client_id=some%20%2A%2A%20key&redirect_uri=http%3A%2F%2Fwww.localhost.me%2F%3Fprovider%3DwindowsLive&response_type=code&scope=wl.signin%2Cwl.basic%2Cwl.emails&state={0}",
                    result.State));
            }
        }

        public class AuthenticateClientAsyncFacts
        {
            [Fact]
            public async Task GivenARepsonse_AuthenticateClientAsync_ReturnsAnAuthenticatedClient()
            {
                // Arramge.
                var callbackUri = new Uri("http://www.localhost.me?provider=windowsLive");
                const string stateKey = "state";
                const string state = "adyiuhj97&^*&shdgf\\//////\\dsf";
                var querystring = new Dictionary<string, string>
                {
                    {stateKey, state},
                    {"code", "4/P7q7W91a-oMsCeLvIaQm6bTrgtp7"}
                };
                var accessTokenJson = File.ReadAllText("Sample Data\\WindowsLive-AccessToken-Content.json");
                var accessTokenResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(accessTokenJson);
                var userInformationJson = File.ReadAllText("Sample Data\\WindowsLive-UserInfo-Content.json");
                var userInformationResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(userInformationJson);
                var messageHandler = new FakeHttpMessageHandler(
                    new Dictionary<string, HttpResponseMessage>
                    {
                        {"https://login.live.com/oauth20_token.srf", accessTokenResponse},
                        {
                            "https://apis.live.net/v5.0/me?access_token=EwCIAq1DBAAUGCCXc8wU/zFu9QnLdZXy+YnElFkAAaQn28y+8VQWmn7VMOVO9u",
                            userInformationResponse
                        }
                    });
                var provider = TestHelpers.GetAuthenticationProvider(TestHelpers.WindowsLiveProvider.Name, messageHandler);

                // Act.
                var result = await provider.AuthenticateClientAsync(querystring,
                    state,
                    callbackUri);

                // Assert.
                result.ProviderName.ShouldBe("WindowsLive");
                result.UserInformation.Gender.ShouldBe(GenderType.Male);
                result.UserInformation.Id.ShouldBe("1234");
                result.UserInformation.Locale.ShouldBe("en-au");
                result.UserInformation.Name.ShouldBe("Tanis Half-Elven");
                result.UserInformation.UserName.ShouldBe(null);
                result.UserInformation.Email.ShouldBe("tanis.half-elven@InnOfLastHope.krynn");
                result.UserInformation.Picture.ShouldBe("https://apis.live.net/v5.0/1234/picture");
                result.RawUserInformation.ShouldNotBe(null);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using FakeItEasy;
using Nancy;
using Nancy.Responses;
using Nancy.SimpleAuthentication;
using Shouldly;
using SimpleAuthentication.Core;
using Xunit;

namespace SimpleAuthentication.Tests.WebSites.Nancy
{
    public class SampleJsonAuthenticationProviderCallbackFacts
    {
        internal static AuthenticatedClient FakeAuthenticatedClient
        {
            get
            {
                var accessToken = new AccessToken
                {
                    ExpiresOn = new DateTime(2020, 5, 23),
                    Token = "B687DAD0-8029-47DB-9BC3-1F2F40624377"
                };
                var userInformation = new UserInformation
                {
                    Email = "sturm.brightblade@KnightsOfTheRose.krynn",
                    Gender = GenderType.Male,
                    Id = "1234A-SB",
                    Name = "Sturm Brightblade",
                    UserName = "Sturm.Brightblade"
                };

                return new AuthenticatedClient("google",
                    accessToken,
                    userInformation,
                    "some raw user information");
            }
        }

        public class ProcessFacts
        {
            [Fact]
            public void GivenAnAuthenticatedClientAndNoReturnUrl_Process_ReturnsAView()
            {
                // Arrange.
                var fakeResponse = A.Fake<IResponseFormatter>();
                A.CallTo(() => fakeResponse.Serializers).Returns(new[] {new DefaultJsonSerializer()});

                var nancyModule = A.Fake<INancyModule>();
                A.CallTo(() => nancyModule.Response)
                    .Returns(fakeResponse);

                var view = new ViewRenderer(nancyModule);
                A.CallTo(() => nancyModule.View).Returns(view);
                var authenticationProviderCallback = new SampleJsonAuthenticationProviderCallback();

                var authenticationCallbackResult = new AuthenticateCallbackResult
                {
                    AuthenticatedClient = FakeAuthenticatedClient
                };

                // Act.
                var result = (Response)authenticationProviderCallback.Process(nancyModule, authenticationCallbackResult);

                // Assert.
                // Not sure how to do this yet.
                result.StatusCode.ShouldBe(HttpStatusCode.OK);
                result.ContentType.ShouldBe("application/json; charset=utf-8");

                // TODO: How to test the model? How can we grab the contents (the json payload)?
            }

            [Fact]
            public void GivenAnAuthenticatedClientAndAReturnUrl_Process_ReturnsARedirection()
            {
                // Arrange.
                var fakeResponse = A.Fake<IResponseFormatter>();
                A.CallTo(() => fakeResponse.Serializers).Returns(new[] { new DefaultJsonSerializer() });

                var nancyModule = A.Fake<INancyModule>();
                A.CallTo(() => nancyModule.Response)
                    .Returns(fakeResponse);

                var view = new ViewRenderer(nancyModule);
                A.CallTo(() => nancyModule.View).Returns(view);
                
                var authenticationProviderCallback = new SampleJsonAuthenticationProviderCallback();

                var authenticationCallbackResult = new AuthenticateCallbackResult
                {
                    AuthenticatedClient = FakeAuthenticatedClient,
                    ReturnUrl = "http://please.go.home/a/b/c?xx=hi"
                };

                // Act.
                var result = (Response)authenticationProviderCallback.Process(nancyModule, authenticationCallbackResult);

                // Assert.
                // Not sure how to do this yet.
                result.StatusCode.ShouldBe(HttpStatusCode.SeeOther);
                result.ContentType.ShouldBe("text/html");
            }

            [Fact(Skip = "TODO")]
            public void GivenNoAuthenticatedClientAndNoReturnUrl_Process_ReturnsARedirection()
            {
            }

            [Fact(Skip = "TODO")]
            public void GivenNoAuthenticatedClientAndAReturnUrl_Process_ReturnsARedirection()
            {
            }
        }
    }
}
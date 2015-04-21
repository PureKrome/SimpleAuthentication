using System;
using System.Linq;
using Shouldly;
using SimpleAuthentication.Core.Config;
using Xunit;

namespace SimpleAuthentication.Tests.Config
{
    public class AppCustomSectionParserFacts
    {
        [Fact]
        public void GivenNoFile_ParseAppCustomSection_ReturnsANullConfiguration()
        {
            // Arrange & Act.
            var configuration = AppCustomSectionParser.ParseAppCustomSection();

            // Assert.
            configuration.ShouldBe(null);
        }

        [Fact]
        public void GivenAMissingFile_ParseAppCustomSection_ReturnsANullConfiguration()
        {
            // Arrange & Act.
            var configuration = AppCustomSectionParser.ParseAppCustomSection(file: "missing.config");

            // Assert.
            configuration.ShouldBe(null);
        }

        [Fact]
        public void GivenAnFileWithNoSection_ParseAppCustomSection_ReturnsANullConfiguration()
        {
            // Arrange & Act.
            var configuration = AppCustomSectionParser.ParseAppCustomSection(file: "Config\\SampleEmptyConfigSettings.xml");

            // Assert.
            configuration.ShouldBe(null);
        }

        [Fact]
        public void GivenAnFileWithAValidSection_ParseAppCustomSection_ReturnsAConfiguration()
        {
            // Arrange & Act.
            var configuration = AppCustomSectionParser.ParseAppCustomSection(file: "Config\\SampleCustomSectionConfigSettings.xml");

            // Assert.
            configuration.Providers.Count.ShouldBe(3);

            var facebook = configuration.Providers.Single(x => x.Name == "Facebook");
            facebook.Name.ShouldBe("Facebook");
            facebook.Key.ShouldBe("289376437845497");
            facebook.Secret.ShouldBe("hihihihi");
            facebook.Scopes.ShouldBe("scope1;scope2");

            var google = configuration.Providers.Single(x => x.Name == "Google");
            google.Name.ShouldBe("Google");
            google.Key.ShouldBe("587140099194.apps.googleusercontent.com");
            google.Secret.ShouldBe("pewpewpewp");
            google.Scopes.ShouldBe(null);
        }

        [Fact]
        public void GivenAnFileWithAnInvalidSection_ParseAppCustomSection_ThrowsAnException()
        {
            // Arrange & Act.
            var exception = Should.Throw<Exception>(
                () => AppCustomSectionParser.ParseAppCustomSection(file: "Config\\SampleInvalidCustomSectionConfigSettings.xml"));

            // Assert.
            exception.Message.ShouldStartWith("Unrecognized attribute 'BadBadBad'. Note that attribute names are case-sensitive.");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shouldly;
using SimpleAuthentication.Core;
using SimpleAuthentication.Core.Config;
using SimpleAuthentication.Core.Providers;
using SimpleAuthentication.ExtraProviders;
using WorldDomination.Net.Http;

namespace SimpleAuthentication.Tests
{
    public static class TestHelpers
    {
        private static readonly Lazy<IList<Provider>> Providers = new Lazy<IList<Provider>>(CreateProviders);
        public const string ConfigProviderKey = "some ** key";
        public const string ConfigProviderSecret = "some secret";

        public static string ToEncodedString(this List<KeyValuePair<string, string>> value)
        {
            var result = new StringBuilder();
            foreach (var keyValuePair in value)
            {
                if (result.Length > 0)
                {
                    result.Append("&");
                }

                result.AppendFormat("{0}={1}", 
                    Uri.EscapeDataString(keyValuePair.Key), 
                    Uri.EscapeDataString(keyValuePair.Value));
            }

            return result.ToString();
        }

        public static Provider GoogleProvider
        {
            get { return Providers.Value.Single(x => x.Name == "google"); }
        }

        public static Provider FacebookProvider
        {
            get { return Providers.Value.Single(x => x.Name == "facebook"); }
        }

        public static Provider TwitterProvider
        {
            get { return Providers.Value.Single(x => x.Name == "twitter"); }
        }

        public static Provider InstagramProvider
        {
            get { return Providers.Value.Single(x => x.Name == "instagram"); }
        }
        
        public static Provider GitHubProvider
        {
            get { return Providers.Value.Single(x => x.Name == "github"); }
        }

        public static Provider WindowsLiveProvider
        {
            get { return Providers.Value.Single(x => x.Name == "windowslive"); }
        }

        public static Configuration ConfigurationProviders
        {
            get
            {
                return new Configuration
                {
                    Providers = Providers.Value
                };
            }
        }

        public static Configuration FilteredConfigurationProviders(string filterKey)
        {
            var configuration = ConfigurationProviders;
            configuration.Providers = configuration.Providers
                .Where(x => x.Name == filterKey)
                .ToList();

            return configuration;
        }

        public static IDictionary<string, IAuthenticationProvider> GetAuthenticationProviders(
            FakeHttpMessageHandler messageHandler = null)
        {
            var providerParams = new ProviderParams(ConfigProviderKey, ConfigProviderSecret);

            return new Dictionary<string, IAuthenticationProvider>
            {
                {
                    GoogleProvider.Name,
                    new GoogleProvider(providerParams, messageHandler)
                },
                {
                    FacebookProvider.Name,
                    new FacebookProvider(providerParams, messageHandler)
                },
                {
                    TwitterProvider.Name,
                    new TwitterProvider(providerParams, messageHandler)
                },
                {
                    InstagramProvider.Name,
                    new InstagramProvider(providerParams, messageHandler)
                },
                {
                    GitHubProvider.Name,
                    new GitHubProvider(providerParams, messageHandler)
                },

                {
                    WindowsLiveProvider.Name,
                    new WindowsLiveProvider(providerParams, messageHandler)
                },
            };
        }

        public static IAuthenticationProvider GetAuthenticationProvider(string key,
            FakeHttpMessageHandler messageHandler = null)
        {
            key.ShouldNotBeNullOrEmpty();

            var providers = GetAuthenticationProviders(messageHandler);
            if (providers.ContainsKey(key))
            {
                return providers[key];
            }

            var errorMessage = string.Format("Provider '{0}' not handled. Please add an if-check to handle this.", key);
            throw new Exception(errorMessage);
        }

        private static IList<Provider> CreateProviders()
        {
            var providers = new List<Provider>
            {
                CreateProvider("google"),
                CreateProvider("facebook"),
                CreateProvider("twitter"),
                CreateProvider("instagram"),
                CreateProvider("github"),
                CreateProvider("windowslive")
            };

            return providers;
        }

        private static Provider CreateProvider(string name)
        {
            name.ShouldNotBeNullOrEmpty();

            return new Provider
            {
                Name = name,
                Key = ConfigProviderKey,
                Secret = ConfigProviderSecret
            };
        }
    }
}
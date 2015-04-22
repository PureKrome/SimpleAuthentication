using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

namespace SimpleAuthentication.Core.Config
{
    public static class AppSettingsParser
    {
        private const string KeyPrefix = "sa.";
        private const string RedirectRouteKey = "redirectroute";
        private const string CallbackRouteKey = "callbackroute";

        public static Configuration ParseAppSettings(string file = null)
        {
            var appSettings = string.IsNullOrWhiteSpace(file)
                ? GetAppSettingsFromConfigFile()
                : GetAppSettingsFromConfigFile(file);

            return appSettings == null
                ? null
                : ParseAppSettingsCollection(appSettings);
        }

        private static NameValueCollection GetAppSettingsFromConfigFile()
        {
            var appSettings = ConfigurationManager.AppSettings;
            return appSettings.Count <= 0
                ? null
                : appSettings;
        }

        private static NameValueCollection GetAppSettingsFromConfigFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException(file);
            }

            var exeFilePath = new ExeConfigurationFileMap
            {
                ExeConfigFilename = file
            };

            var configuration = ConfigurationManager.OpenMappedExeConfiguration(exeFilePath, ConfigurationUserLevel.None);
            if (configuration.AppSettings == null ||
                configuration.AppSettings.Settings == null ||
                configuration.AppSettings.Settings.Count <= 0)
            {
                return null;
            }

            var result = new NameValueCollection();
            foreach (KeyValueConfigurationElement item in configuration.AppSettings.Settings)
            {
                result.Add(item.Key, item.Value);
            }

            return result;
        }

        private static Configuration ParseAppSettingsCollection(NameValueCollection settings)
        {
            var configuration = new Configuration();

            foreach (var key in settings.AllKeys)
            {
                // Example key: <app key="sa.github" value="secret:aaa;key:bbb;scopes:ccc..."/>

                // If it doesn't start with our specific prefix then we ignore it.
                if (!key.StartsWith(KeyPrefix))
                {
                    continue;
                }

                var provider = key.Replace(KeyPrefix.ToLowerInvariant(), string.Empty);
                var value = settings[key];

                // First we need to check for our specific Keys. Otherwise, we assume it's a provider key.
                switch (provider)
                {
                    case RedirectRouteKey:
                        configuration.RedirectRoute = value;
                        break;
                    case CallbackRouteKey:
                        configuration.CallBackRoute = value;
                        break;
                    default:
                        // Fallback - we're assuming the key is a provider...
                        if (configuration.Providers == null)
                        {
                            configuration.Providers = new List<Provider>();
                        }

                        configuration.Providers.Add(ParseValueForProviderData(provider, value));
                        break;
                }
            }

            return configuration;
        }

        private static string GetValue(this IEnumerable<dynamic> values, string key)
        {
            var value = values.FirstOrDefault(x => (string) x.Key == key);

            if (value == null)
            {
                return string.Empty;
            }

            return value.Value;
        }

        private static Provider ParseValueForProviderData(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(key);
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }


            var values = value.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
                {
                    var split = x.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);

                    return new
                    {
                        Key = split[0].ToLower(),
                        Value = split[1]
                    };
                }).ToList();

            const string keyKey = "key";
            const string secretKey = "secret";
            const string scopesKey = "scopes";

            return new Provider
            {
                Name = key,
                Secret = values.Exists(x => x.Key == secretKey)
                    ? values.GetValue(secretKey)
                    : null,
                Key = values.Exists(x => x.Key == keyKey)
                    ? values.GetValue(keyKey)
                    : null,
                Scopes = values.Exists(x => x.Key == scopesKey)
                    ? values.GetValue("scopes")
                    : null
            };
        }
    }
}
using System;
using System.Configuration;
using System.Linq;
using SimpleAuthentication.Core.Exceptions;

namespace SimpleAuthentication.Core.Config
{
    public class AppConfigService : IConfigService
    {
        public Configuration GetConfiguration()
        {
            var configuration = UseAppSettings() ?? UseConfig();

            if (configuration == null)
            {
                throw new AuthenticationException("No AppSettings keys -or- a SimpleAuthentication configuration section was found. At least one of those configration options is required - otherwise, how do we know ~how~ to connect to any Authentication Providers which you have required? A sample <appSettings> key/value is: <add key=\"sa.Google\" value=\"key:blahblahblah.apps.googleusercontent.com;secret:pew-pew\". Please check the documentation about how to add your specific configuration settings./>");
            }

            return configuration;
        }

        private static Configuration UseConfig(string sectionName = "authenticationProviders")
        {
            var configuration = AppCustomSectionParser.ParseAppCustomSection(sectionName);

            return configuration == null ||
                   configuration.Providers == null ||
                   !configuration.Providers.Any()
                ? null
                : configuration;
        }

        private static Configuration UseAppSettings()
        {
            var configuration = AppSettingsParser.ParseAppSettings();

            return configuration == null ||
                   configuration.Providers == null ||
                   !configuration.Providers.Any()
                ? null
                : configuration;
        }
    }
}
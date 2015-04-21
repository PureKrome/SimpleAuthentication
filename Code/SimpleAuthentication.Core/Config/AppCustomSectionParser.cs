using System;
using System.Configuration;
using System.Linq;

namespace SimpleAuthentication.Core.Config
{
    public static class AppCustomSectionParser
    {
        /// <summary>
        /// A configuration instance containing all the provider information and other initial Simple Authentication application settings.
        /// </summary>
        /// <param name="sectionName">(Optional) Custom section name containing the settings. Default: <code>authenticationProviders</code>.</param>
        /// <param name="file">(Optional) The <code>.config</code> file which contains the custom section, etc. Default: the normal application's <code>.config</code>file.</param>
        /// <returns>A (Simple Authentication) configuration instance containing the initial (Simple Authentication) application settings.</returns>
        /// <remarks>The optional <code>file</code> parameter is mainly for testing. Just stick with the default configuration file for all scenario's.</remarks>
        public static Configuration ParseAppCustomSection(string sectionName = "authenticationProviders",
            string file = null)
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                throw new ArgumentNullException(sectionName);
            }

            var exeConfiguration = GetExeConfiguration(file);
            if (exeConfiguration == null)
            {
                return null;
            }

            var configSection = exeConfiguration.GetSection(sectionName) as ProviderConfiguration;
            return configSection == null ||
                   configSection.Providers == null ||
                   configSection.Providers.Count <= 0
                ? null
                : new Configuration
                {
                    RedirectRoute = configSection.RedirectRoute,
                    CallBackRoute = configSection.CallbackRoute,
                    Providers = (from p in configSection.Providers.AllKeys
                        let name = configSection.Providers[p].Name
                        let key = configSection.Providers[p].Key
                        let secret = configSection.Providers[p].Secret
                        let scopes = configSection.Providers[p].Scopes
                        select new Provider
                        {
                            Name = string.IsNullOrWhiteSpace(name) ? null : name,
                            Key = string.IsNullOrWhiteSpace(key) ? null : key,
                            Secret = string.IsNullOrWhiteSpace(secret) ? null : secret,
                            Scopes = string.IsNullOrWhiteSpace(scopes) ? null : scopes
                        }).ToList()
                };
        }

        private static System.Configuration.Configuration GetExeConfiguration(string file = null)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }

            var exeFilePath = new ExeConfigurationFileMap
            {
                ExeConfigFilename = file
            };

            return ConfigurationManager.OpenMappedExeConfiguration(exeFilePath, ConfigurationUserLevel.None);
        }
    }
}
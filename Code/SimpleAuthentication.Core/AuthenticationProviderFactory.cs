﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using SimpleAuthentication.Core.Config;
using SimpleAuthentication.Core.Exceptions;
using SimpleAuthentication.Core.Providers;
using SimpleAuthentication.Core.Tracing;

namespace SimpleAuthentication.Core
{
    public class AuthenticationProviderFactory : IAuthenticationProviderFactory
    {
        private readonly HttpMessageHandler _httpMessageHandler;
        private readonly Lazy<ITraceManager> _traceManager = new Lazy<ITraceManager>(() => new TraceManager());

        public AuthenticationProviderFactory(IConfigService configService,
            IProviderScanner providerScanner) : this(configService, providerScanner, null)
        { }

        public AuthenticationProviderFactory(IConfigService configService,
            IProviderScanner providerScanner,
            HttpMessageHandler httpMessageHandler)
        {
            if (configService == null)
            {
                throw new ArgumentNullException("configService");
            }

            if (providerScanner == null)
            {
                throw new ArgumentNullException("providerScanner");
            }

            // NOTE: Optional message handler. This is used mainly for unit testing purposes.
            _httpMessageHandler = httpMessageHandler;

            TraceSource.TraceVerbose("Authentication provider factory instantiated with {0} message handler.",
                _httpMessageHandler == null
                    ? "no"
                    : "a");

            SetupAuthenticationProviders(configService, providerScanner);
        }

        public IDictionary<string, IAuthenticationProvider> AuthenticationProviders { get; private set; }

        private void SetupAuthenticationProviders(IConfigService configService,
            IProviderScanner providerScanner)
        {
            TraceSource.TraceVerbose("Setup or Scanning for Authentication Providers.");

            if (configService == null)
            {
                throw new ArgumentNullException("configService");
            }

            if (providerScanner == null)
            {
                throw new ArgumentNullException("providerScanner");
            }

            var configuration = configService.GetConfiguration();
            if (configuration == null ||
                configuration.Providers == null ||
                !configuration.Providers.Any())
            {
                throw new AuthenticationException("There needs to be at least one Authentication Provider's detail's in the configService.Provider's collection. Otherwise, how else are we to set the available Authentication Providers?");
            }

            var discoveredProviders = providerScanner.GetDiscoveredProviders();
            if (discoveredProviders == null ||
                !discoveredProviders.Any())
            {
                throw new AuthenticationException("No discovered providers were found by the Provider Scanner. We need at least one IAuthenticationProvider type to exist so we can attempt to map the authentication data (from the configService) to the found Provider.");
            }

            TraceSource.TraceInformation("Discovered {0} Authentication Providers.", discoveredProviders.Count);

            // Lets try and map the developer-provided list of providers against the available
            // list that was discovered.
            // We don't want to load providers that cannot be mapped -- after all,
            // we can only goto a provider if we have the api keys, etc.
            foreach (var configProvider in configuration.Providers)
            {
                var provider = LoadProvider(configProvider, discoveredProviders);
                if (provider == null)
                {
                    throw new AuthenticationException("Failed to create a Provider for the name: {0}. Maybe the concrete class doesn't exist or the constructor params are not the required ones?");
                }

                if (AuthenticationProviders == null)
                {
                    AuthenticationProviders = new Dictionary<string, IAuthenticationProvider>();
                }

                var key = provider.Name.ToLowerInvariant();

                if (!AuthenticationProviders.ContainsKey(key))
                {
                    AuthenticationProviders.Add(key, provider);
                }
            }

            TraceSource.TraceInformation("Mapped {0} providers to configuration settings.", AuthenticationProviders.Count);
        }

        private IAuthenticationProvider LoadProvider(Provider configProvider,
            IList<Type> discoveredProviders)
        {
            if (configProvider == null)
            {
                throw new ArgumentNullException("configProvider");
            }

            if (discoveredProviders == null ||
                !discoveredProviders.Any())
            {
                throw new ArgumentNullException("discoveredProviders");
            }

            var configProviderName = configProvider.Name.ToLowerInvariant();

            var exisitingProvider = discoveredProviders
                .SingleOrDefault(x => x.Name.ToLowerInvariant().StartsWith(configProviderName));

            if (exisitingProvider == null)
            {
                string errorMessage =
                    string.Format(
                        "Unable to find the provider [{0}]. Is there a provider dll available? Is there a typo in the provider name? Solution suggestions: Check to make sure the correct dll's are in the 'bin' directory and/or check the name to make sure there's no typo's in there. Example: If you're trying include the GitHub provider, make sure the name is 'github' (any case) and that the ExtraProviders dll exists in the 'bin' directory or make sure you've downloaded the package via NuGet -> install-package SimpleAuthentication.ExtraProviders.",
                        configProviderName);

                throw new AuthenticationException(errorMessage);
            }

            IAuthenticationProvider authenticationProvider = null;

            var parameters = new ProviderParams(configProvider.Key,
                    configProvider.Secret,
                    configProvider.Scopes.ScopesToCollection());

            // Make sure we have a provider with the correct constructor parameters.
            // How? If a person creates their own provider and doesn't offer a constructor
            // that has a sigle ProviderParams -or- a ProviderParams and MessageHandler, 
            // then we're stuffed. So we need to help them.
            if (exisitingProvider.GetConstructor(new[] { typeof(ProviderParams) }) != null)
            {
                authenticationProvider =
                    Activator.CreateInstance(exisitingProvider, parameters) as IAuthenticationProvider;
            }

            else if (exisitingProvider.GetConstructor(
                new[] {typeof (ProviderParams), typeof (HttpMessageHandler)}) != null)
            {
                authenticationProvider =
                    Activator.CreateInstance(exisitingProvider, parameters, _httpMessageHandler) as
                        IAuthenticationProvider;
            }

            if (authenticationProvider == null)
            {
                // We didn't find a proper constructor for the provider class we wish to instantiate.
                string errorMessage =
                    string.Format(
                        "The type {0} doesn't have the proper constructor. It requires a constructor that only accepts 1 argument of type ProviderParams. Eg. public MyProvider(ProviderParams providerParams){{ .. }}.",
                        exisitingProvider.FullName);

                throw new ApplicationException(errorMessage);
            }

            return authenticationProvider;
        }

        private TraceSource TraceSource
        {
            get { return _traceManager.Value["Nancy.SimpleAuthentication.AuthenticationProviderFactory"]; }
        }
    }

    public static class CollectionExtensions
    {
        public static ICollection<string> ScopesToCollection(this string scopes)
        {
            return string.IsNullOrEmpty(scopes) 
                ? null 
                : scopes.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
        }
    }

    public static class ProvidersExtensions
    {
        public static bool ContainsAnAuthenticationProvider(this IList<IAuthenticationProvider> providers,
            IAuthenticationProvider provider)
        {
            return providers.Any(x => x.Name.Equals(provider.Name, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
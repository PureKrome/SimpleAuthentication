﻿using Nancy;
using Nancy.Bootstrapper;
using Nancy.Session;
using Nancy.SimpleAuthentication;
using Nancy.TinyIoc;
using SimpleAuthentication.Core.Providers;
using SimpleAuthentication.ExtraProviders;
using SimpleAuthentication.Sample.Nancy.Helpers;

namespace SimpleAuthentication.Sample.Nancy
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            // Manually adding our own Types (instead of scanning for them or they exist in a weird location
            // where the scanner can't find them).
            var providers = ProviderScanner.DefaultAndFakeProviders;
            providers.Add(typeof (GitHubProvider));
            providers.Add(typeof (InstagramProvider));

            var providerScanner = new ProviderScanner(providers);
            container.Register<IProviderScanner, ProviderScanner>(providerScanner);
            container.Register<IAuthenticationProviderCallback, SampleNancyAuthenticationCallbackProvider>();

            CookieBasedSessions.Enable(pipelines);
        }
    }
}
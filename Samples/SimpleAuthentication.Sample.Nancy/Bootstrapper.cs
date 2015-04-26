using Nancy;
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

            
            // NOTE: We have an option here to either
            // A: * Use the default ConfigService, which is the AppSettingsService.
            //    * Use the default ProviderScanner -> which is a hardcoded list -> Google, FB, Twitter, WL.
            // -or-
            // B: we manually specify which providers we want and where these settings are to be found.
            // ~~~~ Comment/Uncomment which ever option you want ~~~~~

            // Comment the below line out (which is choice 'B') if you want to test choice 'A'.
            ManuallySpecifyTheAuthenticationProviders(container);


            container.Register<IAuthenticationProviderCallback, SampleNancyAuthenticationCallbackProvider>();

            CookieBasedSessions.Enable(pipelines);
        }

        private static void ManuallySpecifyTheAuthenticationProviders(TinyIoCContainer container)
        {
            // Manually adding our own Types (instead of scanning for them or they exist in a weird location
            // where the scanner can't find them).
            var providers = ProviderScanner.DefaultAndFakeProviders;
            providers.Add(typeof(GitHubProvider));
            providers.Add(typeof(InstagramProvider));

            var providerScanner = new ProviderScanner(providers);
            container.Register<IProviderScanner, ProviderScanner>(providerScanner);
        }
    }
}
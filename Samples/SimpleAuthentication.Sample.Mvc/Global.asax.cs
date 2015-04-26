using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using SimpleAuthentication.Core.Providers;
using SimpleAuthentication.Mvc;
using SimpleAuthentication.Sample.Mvc.Helpers;

namespace SimpleAuthentication.Sample.Mvc
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var builder = new ContainerBuilder();

            // NOTE: We have an option here to either
            // A: * Use the default ConfigService, which is the AppSettingsService.
            //    * Use the default ProviderScanner -> which is a hardcoded list -> Google, FB, Twitter, WL.
            // -or-
            // B: we manually specify which providers we want and where these settings are to be found.
            // ~~~~ Comment/Uncomment which ever option you want ~~~~~

            // Comment the below line out (which is choice 'B') if you want to test choice 'A'.
            ManuallySpecifyTheAuthenticationProviders(builder);

            // We need to define the callback : what do to when we've returned from Google, FB, etc..
            builder.RegisterType<SampleMvcAuthenticationCallbackProvider>().As<IAuthenticationProviderCallback>();

            // Finally, MVC needs us to register the controller. 
            // (This is normal MVC behaviour - nothing unique/specific to Simple Authentication.)
            builder.RegisterControllers(typeof(SimpleAuthenticationController).Assembly);

            // Now complete the DI/IoC setup.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private static void ManuallySpecifyTheAuthenticationProviders(ContainerBuilder container)
        {
            // Manually adding our own Types (instead of scanning for them or they exist in a weird location
            // where the scanner can't find them).
            var providers = ProviderScanner.DefaultAndFakeProviders;
            var providerScanner = new ProviderScanner(providers);
            container.RegisterInstance(providerScanner).SingleInstance();
        }
    }
}
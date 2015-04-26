using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace SimpleAuthentication.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string RedirectToProvider(this UrlHelper url,
            string providerKey,
            string returnUrl = null)
        {
            if (string.IsNullOrEmpty(providerKey))
            {
                throw new ArgumentNullException("providerKey",
                    "Missing a providerKey value. Please provide one (boom tish!) so we know what route to generate.");
            }

            var parameters = new RouteValueDictionary
            {
                {"providerKey", providerKey},
                {"returnUrl", returnUrl}
            };
            
            return url.RouteUrl("SimpleAuthentication.Mvc-Redirect", parameters);
        }
    }
}
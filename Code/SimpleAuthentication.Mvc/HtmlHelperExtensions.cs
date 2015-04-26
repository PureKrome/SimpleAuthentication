﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace SimpleAuthentication.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlString RedirectToProvider(this HtmlHelper htmlHelper,
            string providerKey,
            string innerHtml,
            string returnUrl = null,
            IDictionary<string, object> htmlAttributes = null)
        {
            if (string.IsNullOrEmpty(providerKey))
            {
                throw new ArgumentNullException("providerKey",
                    "Missing a providerKey value. Please provide one so we know what route to generate.");
            }

            if (string.IsNullOrEmpty(innerHtml))
            {
                throw new ArgumentNullException("innerHtml",
                    "Missing an innerHtml value. We need to display some link text or image to be able to click on - so please provide some html. eg. <img src=\"/ContentResult/someButton.png\" alt=\"click me\"/>");
            }

            // Start with an <a /> element.
            var tagBuilder = new TagBuilder("a")
            {
                InnerHtml = innerHtml
            };

            // Merge any optional attributes. For example, class values, etc.
            if (htmlAttributes != null)
            {
                tagBuilder.MergeAttributes(htmlAttributes);
            }

            // Determine the route.
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var url = urlHelper.RedirectToProvider(providerKey, returnUrl);

            // Set the route.
            tagBuilder.MergeAttribute("href", url);

            return new HtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }
    }
}
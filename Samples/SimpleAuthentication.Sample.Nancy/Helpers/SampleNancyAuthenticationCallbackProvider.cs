﻿using Nancy;
using Nancy.SimpleAuthentication;
using SimpleAuthentication.Core;
using SimpleAuthentication.Core.Exceptions;
using SimpleAuthentication.Sample.Nancy.Models;

namespace SimpleAuthentication.Sample.Nancy.Helpers
{
    public class SampleNancyAuthenticationCallbackProvider : IAuthenticationProviderCallback 
    {
        public dynamic Process(INancyModule module, AuthenticateCallbackResult result)
        {
            var model = new AuthenticationViewModel
            {
                AuthenticatedClient = result.AuthenticatedClient,
                ReturnUrl = result.ReturnUrl
            };

            // Usually, magic stuff with a database happens here ...
            // but for this demo, we'll just dump the result back..


            return module.View["AuthenticateCallback", model];

            // --Or--
            // you can use a Negotiator...
            //return new Negotiator(module.Context)
            //    .WithModel(model)
            //    .WithView("AuthenticateCallback");
        }

        public dynamic OnError(INancyModule module, ErrorType errorType, AuthenticationException exception)
        {
            var model = new AuthenticationViewModel
            {
                ErrorMessage =
                    string.Format(
                        "{0} -- You didn't accept the Scopes which this application has requested (which is totally fine) -or- something crazy happened between you and the Authentication Provider.",
                        exception.Message)
            };

            //return nancyModule.Response.AsRedirect("/");

            if (errorType != ErrorType.UserInformation)
            {
                return module.View["AuthenticateCallback", model];
            }

            var errorModel = new
            {
                errorMessage = exception.Message
            };

            return module.Response.AsJson(errorModel, (HttpStatusCode) exception.HttpStatusCode);
        }
    }
}
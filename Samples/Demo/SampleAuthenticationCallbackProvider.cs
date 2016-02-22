using Nancy;
using Nancy.SimpleAuthentication;

namespace Demo
{
    public class SampleAuthenticationCallbackProvider : IAuthenticationCallbackProvider
    {
        public dynamic Process(NancyModule nancyModule, AuthenticateCallbackData model)
        {
            return "Authenticated";
        }

        public dynamic OnRedirectToAuthenticationProviderError(NancyModule nancyModule, string errorMessage)
        {
            return $"Errored: {errorMessage}";
        }
    }
}
using System;
using System.Collections.Generic;

namespace SimpleAuthentication.Core.Providers
{
    public class ProviderScanner : IProviderScanner
    {
        private readonly IList<Type> _explicitProviders;

        public ProviderScanner() : this(DefaultProviders)
        {
        }

        public ProviderScanner(IList<Type> providers)
        {
            // Optional.
            _explicitProviders = providers;
        }

        /// <summary>
        /// A common list of providers: Google, Facebook, Twitter, WindowsLive.
        /// </summary>
        public static IList<Type> DefaultProviders
        {
            get
            {
                return new List<Type>
                {
                    typeof (GoogleProvider),
                    typeof (FacebookProvider),
                    typeof (TwitterProvider),
                    typeof (WindowsLiveProvider)
                };
            }
        }

        /// <summary>
        /// A common list of providers, including a Fake provider.
        /// </summary>
        /// <remarks><b>Note:</b> This includes the <b>FAKE</b> provider (which is great!) but please don't use this in production. ... Just think 'why' I'm warning you, for a sec ... :)</remarks>
        public static IList<Type> DefaultAndFakeProviders
        {
            get
            {
                var list = DefaultProviders;
                list.Add(typeof (FakeProvider));

                return list;
            }
        }

        /// <summary>
        /// A list of providers.
        /// </summary>
        /// <remarks>This does <b>NOT</b> do any file system scanning. It's purely what has been provided via the ctor.</remarks>
        /// <returns></returns>
        public IList<Type> GetDiscoveredProviders()
        {
            return _explicitProviders;
        }
    }
}
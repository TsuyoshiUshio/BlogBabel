using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelLibs.Resources
{
    public class ProviderFactory
    {
        private readonly IEnumerable<IProvider> _providers;
        public ProviderFactory(IEnumerable<IProvider> providers)
        {
            _providers = providers;
        }

        public ISourceProvider GetSourceProvider(string providerName)
        {
            return _providers.FirstOrDefault(p => typeof(ISourceProvider).IsAssignableFrom(p.GetType()) && p.GetType().Name == $"{providerName}Provider") as ISourceProvider;
        }

        public IDestinationProvider GetDestinationProvider(string providerName)
        {
            return _providers.FirstOrDefault(p => typeof(IDestinationProvider).IsAssignableFrom(p.GetType()) && p.GetType().Name == $"{providerName}Provider") as IDestinationProvider;
        }

    }
}

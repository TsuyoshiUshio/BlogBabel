using BabelLibs.LanguageModels.OpenAI;
using BabelLibs.Resources;
using BabelLibs.Resources.DevTo;
using BabelLibs.Resources.Qiita;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelLibs
{
    public class Processor
    {
        private readonly ProviderFactory _factory;
        private readonly OpenAIProvider _provider;
        private readonly ExecutionContext _context;

        // TODO Start with Simple Plain implementation. Then refactor to separate the responsibility.
        // For the first iteration, it will be experimental implementation for the spike.
        public Processor(ProviderFactory factory, OpenAIProvider provider, ExecutionContext context)
        {
            _factory = factory;
            _provider = provider;
            _context = context;
        }

        public async Task ExecuteAsync()
        {
            var source = _factory.GetSourceProvider(_context.sourceProvider);
            var destination = _factory.GetDestinationProvider(_context.destinationProvider);

            var post = await source.GetPostAsync(_context.sourceIdentifier);
            var translation = await _provider.TranslateAsync(post, destination.TargetLanguage());
            Console.WriteLine($"Title: {translation.Title} \nBody:\n {translation.Body} \nTags: {string.Join(',', translation.Tags)}");
            if (translation != null && destination != null) {
                await destination.PostAsync(translation);
            }


        }
    }
}

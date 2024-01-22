using BabelLibs.LanguageModels.OpenAI;
using BabelLibs.Resources;
using BabelLibs.Resources.DevTo;
using BabelLibs.Resources.Qiita;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly ILogger _logger;

        // TODO Start with Simple Plain implementation. Then refactor to separate the responsibility.
        // For the first iteration, it will be experimental implementation for the spike.
        public Processor(ProviderFactory factory, OpenAIProvider provider, ExecutionContext context, ILogger<Processor> logger)
        {
            _factory = factory;
            _provider = provider;
            _context = context;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var source = _factory.GetSourceProvider(_context.sourceProvider);
            var destination = _factory.GetDestinationProvider(_context.destinationProvider);

            Console.WriteLine($"Start translation from {_context.sourceProvider}({source.GetLanguage()}) to {_context.destinationProvider}({destination.GetLanguage()})");
            var post = await source.GetPostAsync(_context.sourceIdentifier);
            var translation = await _provider.TranslateAsync(post, destination.GetLanguage());
            _logger.LogDebug($"Title: {translation.Title} \nBody:\n {translation.Body} \nTags: {string.Join(',', translation.Tags)}");
            if (translation != null && destination != null) {
                var response = await destination.PostAsync(translation);
                response?.EnsureSuccessStatusCode();
            }
            stopwatch.Stop();
            Console.WriteLine($"Translation has been completed. Elapsed time: {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}

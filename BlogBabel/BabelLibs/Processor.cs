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
        private readonly QiitaProvider _source;
        private readonly DevToProvider _destination;
        private readonly OpenAIProvider _provider;

        // TODO Start with Simple Plain implementation. Then refactor to separate the responsibility.
        // For the first iteration, it will be experimental implementation for the spike.
        public Processor(QiitaProvider source, DevToProvider destination, OpenAIProvider provider)
        {
            _source = source;
            _destination = destination;
            _provider = provider;
        }

        public async Task ExecuteAsync()
        {
            var post = await _source.GetPostAsync("b42773afaa4a25c2af60");
            var result = await _provider.TranslateAsync(post, "English");
            Console.WriteLine($"Title: {result.Title} \nBody:\n {result.Body} \nTags: {string.Join(',', result.Tags)}");
        }
    }
}

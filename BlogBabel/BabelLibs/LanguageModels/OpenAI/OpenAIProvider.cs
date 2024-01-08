using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelLibs.LanguageModels.OpenAI
{
    public class OpenAIProvider
    {
        private readonly OpenAIClient _client;
        public OpenAIProvider(IOptions<OpenAISettings> settings) {
            _client = new OpenAIClient(settings.Value.ApiKey);
        }

        public async Task<Post> TranslateAsync(Post post, string language)
        {
            // Translate the post to the destination language.
            // Try to spike https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/openai/Azure.AI.OpenAI
            var option = new ChatCompletionsOptions
                {
                    DeploymentName = "gpt-3.5-turbo",
                    Messages =
                    {
                        new ChatRequestSystemMessage("You are a bilingal technical blogger. You can translate anything with keeping the context and sentiment."),
                        new ChatRequestUserMessage($"Could you translate the following blogs into {language}? \n {post.Body}"),
                    }
                };

            // TODO Next Step. It will exceed the Usage limit. https://platform.openai.com/account/limits 
            // Do something to avoid the limit.
            var completion = await _client.GetChatCompletionsAsync(option);

            return new Post
            {
                Title = post.Title,
                Body = completion.Value.ToString()
            };
        }
    }
}

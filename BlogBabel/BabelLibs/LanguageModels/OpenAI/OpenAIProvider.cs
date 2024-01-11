using Azure.AI.OpenAI;
using Microsoft.DeepDev;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<OpenAIProvider> _logger;

        public OpenAIProvider(IOptions<OpenAISettings> settings, ILogger <OpenAIProvider> logger) {
            _client = new OpenAIClient(settings.Value.ApiKey);
            _logger = logger;
        }

        public async Task<Post> TranslateAsync(Post post, string language)
        {
            _logger.LogInformation($"Start Translation for {language}.");
            int limit = 2048;
            int tokens = await CountTokens(post.Body);
            _logger.LogInformation($"Count Token has been completed. {tokens} tokens found.");

            var chunks = await Split(post.Body, tokens, limit, 3000);
            _logger.LogInformation($"Split has been completed. {chunks.Count} chunks found.");

            string model = "gpt-3.5-turbo";
            // Translate the post to the destination language.
            // Try to spike https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/openai/Azure.AI.OpenAI
            StringBuilder builder = new StringBuilder();
            Azure.Response<ChatCompletions> titleCompletion = null;
            for(int i = 0; i < chunks.Count; i++)
            {
                var option = new ChatCompletionsOptions
                {
                    DeploymentName = model,
                    Messages =
                    {
                        new ChatRequestSystemMessage("You are a bilingal technical blogger. You can translate anything with keeping the context, sentiment and Markdown format."),
                        new ChatRequestUserMessage($"Could you translate the following blogs into {language} that is the part {i} out of {chunks.Count}? \n {chunks[i]}"),
                    }
                };

                // TODO Next Step. It will exceed the Usage limit. https://platform.openai.com/account/limits 
                // Do something to avoid the limit.
                var completion = await _client.GetChatCompletionsAsync(option);
                var body = completion.Value.Choices[0].Message.Content;

                _logger.LogInformation($"Translation chunk {i} has been completed. {body.Length} characters found.");
                
                builder.AppendLine(body);
            }

            var titleOption = new ChatCompletionsOptions
            {
                DeploymentName = model,
                Messages =
                    {
                        new ChatRequestSystemMessage("You are a bilingal technical blogger. You can translate anything with keeping the context, sentiment and Markdown format."),
                        new ChatRequestUserMessage($"Could you translate the blog title into {language} ? \n {post.Title}"),
                    }
            };
            titleCompletion = await _client.GetChatCompletionsAsync(titleOption);

            _logger.LogInformation($"Translation of the title has been completed. {titleCompletion.Value.Choices[0].Message.Content.Length} characters found.");

            return new Post
            {
                Title = titleCompletion.Value.Choices[0].Message.Content,
                Body = builder.ToString()
            };
        }

        private async Task<IList<string>> Split(string text, int number, int limit, int maxLimit)
        {
            // Write Logic Read text for each line, calcl the number of tokens for each line. 
            // If the number of tokens exceeds the limit, find the next line that has #, then split it as the next string group.
            // Finally return the list of strigs.
            
            using (StringReader reader = new StringReader(text))
            {
                string line;
                int currentToken = 0;
                var currentBuffer = new StringBuilder();
                List<string> result = new List<string>();
                bool findingNextChapter = false;
                while ((line = reader.ReadLine()) != null)
                {
                    int token = await CountTokens(line);
                    currentToken += token;
                    if (currentToken > limit)
                    {
                        findingNextChapter = true;

                        if (line.StartsWith('#') || currentToken > maxLimit)
                        {
                            findingNextChapter = false;
                            currentToken = 0;
                            result.Add(currentBuffer.ToString());
                            currentBuffer.Clear();
                            currentBuffer.AppendLine(line);
                        } 
                        else
                        {
                            currentBuffer.AppendLine(line);
                        }
                    }
                    else
                    {
                        currentBuffer.AppendLine(line);
                    }
                }

                if (currentBuffer.Length != 0)
                {
                    result.Add(currentBuffer.ToString());
                }
                return result;
            }
        }

        private async Task<int> CountTokens(string text)
        {
            var tokenizer  = await TokenizerBuilder.CreateByModelNameAsync("gpt-3.5-turbo");
            var encoded = tokenizer.Encode(text, Array.Empty<string>());
            return encoded.Count;
        }
    }
}

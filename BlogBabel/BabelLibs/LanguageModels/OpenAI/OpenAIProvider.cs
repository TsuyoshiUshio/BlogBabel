﻿using Azure.AI.OpenAI;
using Microsoft.DeepDev;
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
            int limit = 2048;
            int tokens = await CountTokens(post.Body);

            var chunks = await Split(post.Body, tokens, limit, 3000);

            // Translate the post to the destination language.
            // Try to spike https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/openai/Azure.AI.OpenAI
            var option = new ChatCompletionsOptions
                {
                    DeploymentName = "gpt-3.5-turbo",
                    Messages =
                    {
                        new ChatRequestSystemMessage("You are a bilingal technical blogger. You can translate anything with keeping the context, sentiment and Markdown format."),
                        new ChatRequestUserMessage($"Could you translate the following blogs into {language}? \n {chunks.FirstOrDefault()}"),
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
                        currentBuffer.AppendLine(line);
                        if (line.StartsWith('#') || currentToken > maxLimit)
                        {
                            findingNextChapter = false;
                            currentToken = 0;
                            result.Add(currentBuffer.ToString());
                            currentBuffer.Clear();
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
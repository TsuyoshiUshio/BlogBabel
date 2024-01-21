using Azure.AI.OpenAI;
using Microsoft.DeepDev;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace BabelLibs.LanguageModels.OpenAI
{
    public class OpenAIProvider
    {
        private readonly OpenAIClient _client;
        private readonly ILogger<OpenAIProvider> _logger;
        private readonly BlogBabelSettings _settings;
        private readonly ExecutionContext _context;

        public OpenAIProvider(IOptions<OpenAISettings> settings, IOptions<BlogBabelSettings> blogBabelSettings, ExecutionContext executionContext, ILogger <OpenAIProvider> logger) {
            _client = new OpenAIClient(settings.Value.ApiKey);
            _logger = logger;
            _settings = blogBabelSettings.Value;
            _context = executionContext;
        }

        public async Task<Post> TranslateAsync(Post post, string language)
        {
            _logger.LogInformation($"Start Translation for {language}.");
            int limit = 2000;
            int maxLimit = 3000;

            if (_settings.TokenLimit != 0)
            {
                limit = _settings.TokenLimit;
            }

            if (_settings.MaxTokenLimit != 0)
            {
                maxLimit = _settings.MaxTokenLimit;
            }

            if (_context.tokenLimit != 0)
            {
                limit = _context.tokenLimit;
            }

            if (_context.maxTokenLimit != 0)
            {
                maxLimit = _context.maxTokenLimit;
            }

            int tokens = await CountTokens(post.Body);
            _logger.LogInformation($"Count Token has been completed. {tokens} tokens found.");

            var chunks = await Split(post.Body, tokens, limit, maxLimit);
            _logger.LogInformation($"Split has been completed. {chunks.Count} chunks found.");

            string model = _context.modelOption;
            // Translate the post to the destination language.
            // Try to spike https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/openai/Azure.AI.OpenAI
            StringBuilder builder = await TranslateBodyAsync(language, chunks, model);

            Azure.Response<ChatCompletions> titleCompletion = await TranslateTitleAsync(post, language, model);

            Azure.Response<ChatCompletions> tagsCompletion = await TranslateTagAsync(post, language, model);

            return new Post
            {
                Title = titleCompletion.Value.Choices[0].Message.Content,
                Body = builder.ToString(),
                Tags = tagsCompletion.Value.Choices[0].Message.Content.Split(',').ToList()
            };
        }

        private async Task<StringBuilder> TranslateBodyAsync(string language, IList<string> chunks, string model)
        {
            // We split it into chunk since we have the usage limitations:  https://platform.openai.com/account/limits 

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < chunks.Count; i++)
            {
                var option = new ChatCompletionsOptions
                {
                    DeploymentName = model,
                    Messages =
                    {
                        new ChatRequestSystemMessage("You are a bilingal technical blogger. You can translate anything with keeping the context, sentiment and Markdown format. Do not add your comment."),
                        new ChatRequestUserMessage($"Could you translate the following blogs into {language} that is the part {i} out of {chunks.Count}? \n {chunks[i]}"),
                    },
                    Temperature = _context.tempature
                };

                var completion = await _client.GetChatCompletionsAsync(option);
                var body = completion.Value.Choices[0].Message.Content;

                _logger.LogInformation($"Translation chunk {i} has been completed. {body.Length} characters found.");

                builder.AppendLine(body);
            }

            return builder;
        }

        private Task<Azure.Response<ChatCompletions>> TranslateTitleAsync(Post post, string language, string model)
        {
            return GenericTranslateAsync("title", post.Title, language, model);
        }

        private Task<Azure.Response<ChatCompletions>> TranslateTagAsync(Post post, string language, string model)
        {
            return GenericTranslateAsync(
                "tags", 
                string.Join(',', post.Tags), 
                language, 
                model, 
                systemPrompt: "You are a bilingal technical blogger. You can translate anything with keeping the context, sentiment with keeping the same format and the same number of tags. You don't need to add double quote. Tags are comma separeted format. Do not add your comment.",
                userPromptTemplate: "Could you translate the {0} into {1} and if the {0} is English, return the original tags? \n {2}");
        }

        private async Task<Azure.Response<ChatCompletions>> GenericTranslateAsync(
            string topic, 
            string contents, 
            string language, 
            string model,
            string systemPrompt = "You are a bilingal technical blogger. You can translate anything with keeping the context, sentiment, number of tags and Markdown format. Reply answer only.",
            string userPromptTemplate = "Could you translate the {0} into {1}? \n {2}")
        {
            var builder = new StringBuilder();
            builder.AppendFormat(userPromptTemplate, topic, language, contents);
            var userPrompt = builder.ToString();
            var option = new ChatCompletionsOptions
            {
                DeploymentName = model,
                Messages =
                    {
                        new ChatRequestSystemMessage(systemPrompt),
                        new ChatRequestUserMessage(userPrompt),
                    },
                Temperature = _context.tempature
            };
            Azure.Response<ChatCompletions> completion = await _client.GetChatCompletionsAsync(option);

            _logger.LogInformation($"Translation of the {topic} has been completed. {completion.Value.Choices[0].Message.Content.Length} characters found.");
            return completion;
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
                            if (currentToken > maxLimit)
                            {
                                Console.WriteLine($"The number of tokens exceeds the max limit({maxLimit}). {currentToken} tokens found.");
                            }

                            findingNextChapter = false;
                            currentToken = 0;
                            result.Add(currentBuffer.ToString());
                            currentBuffer.Clear();
                            currentBuffer.AppendLine();
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
            var tokenizer  = await TokenizerBuilder.CreateByModelNameAsync(_context.modelOption);
            var encoded = tokenizer.Encode(text, Array.Empty<string>());
            return encoded.Count;
        }
    }
}

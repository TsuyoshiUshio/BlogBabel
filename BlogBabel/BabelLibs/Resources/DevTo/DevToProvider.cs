using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BabelLibs.Resources.DevTo
{
    public class DevToProvider
    {
        private readonly HttpClient _httpClient;
        private readonly DevToSettings _settings;
        public DevToProvider(IOptions<DevToSettings> settings)
        {
            _settings = settings.Value;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("api-key", $"{_settings.ApiKey}");
        }

        public async Task PostAsync(GenericPost post)
        {
            var content = new StringContent(JsonSerializer.Serialize(new
            {
                article = new
                {
                    post.Title,
                    body_markdown = post.Body,
                    published = false,
                    description = post.Title,
                    tags = new[] { _settings.Sentiment }
                }
            }), Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync("https://dev.to/api/articles", content);
            response.EnsureSuccessStatusCode();
        }
    }
}

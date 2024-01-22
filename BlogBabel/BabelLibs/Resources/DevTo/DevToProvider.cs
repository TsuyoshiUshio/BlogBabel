using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BabelLibs.Resources.DevTo
{
    public class DevToProvider : ISourceProvider, IDestinationProvider
    {
        private readonly HttpClient _httpClient;
        private readonly DevToSettings _settings;
        private readonly ILogger _logger;
        public DevToProvider(IOptions<DevToSettings> settings, ILogger<DevToProvider> logger)
        {
            _settings = settings.Value;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("api-key", $"{_settings.ApiKey}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "BlogBabel");
            _logger = logger;
        }

        public async Task<Post> GetPostAsync(string itemId)
        {
            using var response = await _httpClient.GetAsync($"https://dev.to/api/articles/{itemId}");
            response.EnsureSuccessStatusCode();
            using var stream = await response.Content.ReadAsStreamAsync();
            var json = JsonNode.Parse(stream);
            var tags = json["tags"].AsArray().Select(x => x.ToString()).ToList();
            return new Post
            {
                Title = json["title"].ToString(),
                Body = json["body_markdown"].ToString(),
                Tags = tags
            };
        }

        public async Task<HttpResponseMessage?> PostAsync(Post post)
        {
            var json = (JsonSerializer.Serialize(new
            {
                article = new
                {
                    title = post.Title,
                    body_markdown = post.Body,
                    published = false,
                    series = (string)null,
                    main_image = (string)null,
                    canonical_url = (string)null,
                    description = post.Title,
                    tags = post.Tags,
                    organization_id = (string)null
                }
            }));
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var uri = "https://dev.to/api/articles";
            using var response = await _httpClient.PostAsync(uri, content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Failed Post request. uri: {uri} body: {json}");
            }
            response.EnsureSuccessStatusCode();
            Console.WriteLine("Successfully published.");
            return response;
        }

        public string GetLanguage()
        {
            return _settings.Language;
        }
    }
}

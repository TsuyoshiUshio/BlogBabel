using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BabelLibs.Resources.Qiita
{
    public class QiitaProvider : ISourceProvider, IDestinationProvider
    {
        private readonly HttpClient _httpClient;
        private readonly QiitaSettings _settings;
        private readonly ILogger _logger;
        public QiitaProvider(IOptions<QiitaSettings> settings, ILogger<QiitaProvider> logger)
        {
            _settings = settings.Value;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.PersonalAccessToken}");
            _logger = logger;
        }

        public string GetLanguage()
        {
            return _settings.Language;
        }

        public async Task<Post> GetPostAsync(string itemId)
        {
            using var response = await _httpClient.GetAsync($"https://qiita.com/api/v2/items/{itemId}");
            using var stream = await response.Content.ReadAsStreamAsync();
            var json = JsonNode.Parse(stream);
            var tags = json["tags"].AsArray().Select(x => x["name"].ToString()).ToList();
            return new Post
            {
                Title = json["title"].ToString(),
                Body = json["body"].ToString(),
                Tags = tags
            };
        }

        public async Task<HttpResponseMessage?> PostAsync(Post post)
        {
            var qiitaPost = new QiitaPost
            {
                Title = post.Title,
                Body = post.Body,
                Private = true,
                Tags = post.Tags.Select(x => new Tag
                {
                    Name = x,
                    Versions = new List<string>() { "0.0.1" }
                }).ToList()
            };
            var json = JsonSerializer.Serialize(qiitaPost);
            var uri = "https://qiita.com/api/v2/items";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Failed Post request. uri: {uri} body: {json}");
            }
            return response;
        }

        private class QiitaPost
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }
            [JsonPropertyName("body")]
            public string Body { get; set; }
            [JsonPropertyName("tags")]
            public List<Tag> Tags { get; set; }

            [JsonPropertyName("private")]
            public bool Private { get; set; }
        }

        private class Tag
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("versions")]
            public List<string> Versions { get; set; }
        }
    }
}

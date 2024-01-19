using Microsoft.Extensions.Options;
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
        public QiitaProvider(IOptions<QiitaSettings> settings)
        {
            _settings = settings.Value;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.PersonalAccessToken}");
        }

        public string TargetLanguage()
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

        public Task<HttpResponseMessage?> PostAsync(Post post)
        {
            var json = new QiitaPost
            {
                Title = post.Title,
                Body = post.Body,
                Private = false,
                Tags = post.Tags.Select(x => "{\"name\"=>\"" + x + "\", \"versions\"=>[\"0.0.1\"]}").ToList()
            };
            var content = new StringContent(JsonSerializer.Serialize(json), Encoding.UTF8, "application/json");
            return _httpClient.PostAsync("https://qiita.com/api/v2/items", content);
        }

        private class QiitaPost
        {
            public string Title { get; set; }
            public string Body { get; set; }
            public List<string> Tags { get; set; }

            [JsonPropertyName("private")]
            public bool Private { get; set; }
        }
    }
}

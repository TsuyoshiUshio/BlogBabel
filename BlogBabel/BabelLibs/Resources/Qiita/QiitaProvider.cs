using Microsoft.Extensions.Options;
using System.Text.Json.Nodes;

namespace BabelLibs.Resources.Qiita
{
    public class QiitaProvider
    {
        private readonly HttpClient _httpClient;
        private readonly QiitaSettings _settings;
        public QiitaProvider(IOptions<QiitaSettings> settings)
        {
            _settings = settings.Value;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.PersonalAccessToken}");
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
    }
}

using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BabelLibs.Resources.DevTo
{
    public class DevToProvider : IDestinationProvider
    {
        private readonly HttpClient _httpClient;
        private readonly DevToSettings _settings;
        public DevToProvider(IOptions<DevToSettings> settings)
        {
            _settings = settings.Value;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("api-key", $"{_settings.ApiKey}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "BlogBabel");
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

            using var response = await _httpClient.PostAsync("https://dev.to/api/articles", content);
            response.EnsureSuccessStatusCode();
            Console.WriteLine("Successfully published.");
            return response;
        }
    }
}

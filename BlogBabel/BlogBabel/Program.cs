using BabelLibs;
using BabelLibs.LanguageModels.OpenAI;
using BabelLibs.Resources.DevTo;
using BabelLibs.Resources.Qiita;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Authentication.ExtendedProtection;

namespace BlogBabel
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Write down the spike in here. Avoid commiting the secrets.
            IConfiguration configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile(path: "config.json")
                 .Build();

           var services = new ServiceCollection();
            services.Configure<QiitaSettings>(configuration.GetSection($"Providers:{QiitaSettings.SectionName}"));
            services.Configure<DevToSettings>(configuration.GetSection($"Providers:{DevToSettings.SectionName}"));
            services.Configure<OpenAISettings>(configuration.GetSection($"Providers:{OpenAISettings.SectionName}"));
            services.AddSingleton<OpenAIProvider>();
            services.AddSingleton<QiitaProvider>();
            services.AddSingleton<DevToProvider>();
            services.AddSingleton<Processor>();

            var serviceProvider = services.BuildServiceProvider();

            var processor = serviceProvider.GetService<Processor>();
            await processor.ExecuteAsync();
        }
    }
}

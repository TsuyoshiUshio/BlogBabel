using BabelLibs;
using BabelLibs.LanguageModels.OpenAI;
using BabelLibs.Resources;
using BabelLibs.Resources.DevTo;
using BabelLibs.Resources.Qiita;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Security.Authentication.ExtendedProtection;

namespace BlogBabel
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = "BlogBabel";

            var sourceOption = new Option<string>(
                aliases: new[] { "--src", "-s" },
                description: "Source Provider Name. Available Qiita, DevTo")
            { IsRequired = true };

            var sourceIdentifierOption = new Option<string>(
                aliases: new[] { "--src-id", "-si" },
                description: "Source Provider Blog Post Identifier. The value is depend on the provider.")
            { IsRequired = true };

            var destinationOption = new Option<string>(
                aliases: new[] { "--dest", "-d" },
                description: "Destination Provider Name. Available Qiita, DevTo")
            { IsRequired = true };

            var loggingOption = new Option<bool>(
                aliases: new[] { "--verbose", "-v" },
                description: "Enable detail logging.");

            var modelOption = new Option<string>(
                aliases: new[] { "--model", "-m" },
                description: "Language Model Name. Available GPT3, GPT2",
                getDefaultValue: () => "gpt3.5-turbo");

            var tokenLimitOption = new Option<int>(
                aliases: new[] { "--token-limit", "-tl" },
                description: "Token Limit for the language model.",
                getDefaultValue: () => 2000);

            var maxTokenLimitOption = new Option<int>(
                aliases: new[] { "--max-token-limit", "-mtl" },
                description: "Max Token Limit for the language model.",
                getDefaultValue: () => 3000);

            var temperture = new Option<long>(
                aliases: new[] { "--temperture", "-t" },
                description: "Temperture for the language model.",
                getDefaultValue: () => 0);

            var rootCommand = new RootCommand(description: "Blog Babel - Translate your blog into any languages.");
            rootCommand.AddOption(sourceOption);            
            rootCommand.AddOption(sourceIdentifierOption);
            rootCommand.AddOption(destinationOption);
            rootCommand.AddOption(loggingOption);
            rootCommand.AddOption(modelOption);
            rootCommand.AddOption(tokenLimitOption);
            rootCommand.AddOption(maxTokenLimitOption);
            rootCommand.AddOption(temperture);

            BabelLibs.ExecutionContext context = default;
            rootCommand.SetHandler((src, srcId, dst, verbose, model, tokenLimit, maxTokenLimit, tempature) =>
            {
                context = new BabelLibs.ExecutionContext
                (
                    sourceProvider: src,
                    sourceIdentifier: srcId,
                    destinationProvider: dst,
                    loggingOption: verbose,
                    modelOption: model,
                    tokenLimit: tokenLimit,
                    maxTokenLimit: maxTokenLimit,
                    tempature: tempature
                );
            }, sourceOption, sourceIdentifierOption, destinationOption, loggingOption, modelOption, tokenLimitOption, maxTokenLimitOption, temperture);


            await rootCommand.InvokeAsync(args);
            
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine($"Blog Babel - Version {version}");

            // Write down the spike in here. Avoid commiting the secrets.
            IConfiguration configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile(path: "config.json")
                 .Build();
            
            IServiceCollection services = new ServiceCollection();
            services.Configure<QiitaSettings>(configuration.GetSection($"Providers:{QiitaSettings.SectionName}"));
            services.Configure<DevToSettings>(configuration.GetSection($"Providers:{DevToSettings.SectionName}"));
            services.Configure<OpenAISettings>(configuration.GetSection($"Providers:{OpenAISettings.SectionName}"));
            services.Configure<BlogBabelSettings>(configuration.GetSection($"BlogBabel"));
            services.AddSingleton<BabelLibs.ExecutionContext>(context);
            services.AddSingleton<OpenAIProvider>();
            services.AddSingleton<IProvider, QiitaProvider>();
            services.AddSingleton<IProvider, DevToProvider>();
            services.AddSingleton<ProviderFactory>();
            services.AddSingleton<Processor>();
            services.AddLogging(builder =>
            {
                builder.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.fffffff] ";
                });
            });

            var serviceProvider = services.BuildServiceProvider();

            var processor = serviceProvider.GetService<Processor>();
            await processor.ExecuteAsync();
        }
    }
}

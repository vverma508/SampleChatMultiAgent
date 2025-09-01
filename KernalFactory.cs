
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using OpenAI;
using System.ClientModel;

namespace SampleChatMultiAgent
{
    public static class KernalFactory
    {
        public static Kernel GetKernel()
        {
            string modelName = "gpt-4o";

            // Add configuration support
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>() // or use your startup class
                .Build();

            var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(modelName, configuration.GetValue<string>("AzureFoundaryProjectUrl"), configuration.GetValue<string>("AzureFoundaryProjectKey"));

            // Add enterprise components
            builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

            // Build the kernel
            Kernel kernel = builder.Build();

            return kernel;
        }

        public static Kernel GetKernel2()
        {
            string modelName = "gpt-4o";

            // Add configuration support
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>() // or use your startup class
                .Build();

            var builder = Kernel.CreateBuilder();//.AddAzureOpenAIChatCompletion(modelName, configuration.GetValue<string>("AzureFoundaryProjectUrl"), configuration.GetValue<string>("AzureFoundaryProjectKey"));


            var options = new OpenAIClientOptions()
            {
                Endpoint = new Uri(configuration.GetValue<string>("AzureFoundaryProjectUrl"))
            };

            var creds= new ApiKeyCredential(configuration.GetValue<string>("AzureFoundaryProjectKey"));
            var client = new OpenAIClient(creds, options);
            builder.AddOpenAIChatCompletion(modelName,client);

            // Add enterprise components
            builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Debug));

            // Build the kernel
            Kernel kernel = builder.Build();

            return kernel;
        }

    }
}

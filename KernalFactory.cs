using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
      
    }
}

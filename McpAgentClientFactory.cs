using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using ModelContextProtocol.Client;

namespace SampleChatMultiAgent
{
    public static class McpAgentClientFactory
    {
        public static async Task<Kernel> CreateMcpClientAsync()
        {

            await using IMcpClient mcpClient = await McpClientFactory.CreateAsync(new StdioClientTransport(new()
            {
                Command = "dotnet run",
                Arguments = ["--project", "C:\\Workspace\\C#\\AI\\SampleChatMcpServer"],
                Name = "Minimal MCP Server",
            }));

            await mcpClient.PingAsync();
            var tools = await mcpClient.ListToolsAsync();

            // Add configuration support
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>() // or use your startup class
                .Build();

            IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Plugins.AddFromFunctions("Tools", tools.Select(aiFunction => aiFunction.AsKernelFunction()));

            // Add enterprise components
            kernelBuilder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Debug));

            string modelName = "gpt-4o";

            kernelBuilder.AddAzureOpenAIChatCompletion(modelName, configuration.GetValue<string>("AzureFoundaryProjectUrl"), configuration.GetValue<string>("AzureFoundaryProjectKey"));

            Kernel kernel = kernelBuilder.Build();
            return kernel;
           
        }
    }
}

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.Client;
using OpenAI;
using System.ClientModel;

namespace SampleChatMultiAgent
{
    public static class McpAgentClientFactory
    {
        public static async Task CreateMcpClientAsync()
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

            var options = new OpenAIClientOptions()
            {
                Endpoint = new Uri(configuration.GetValue<string>("AzureFoundaryProjectUrl"))
            };

            string modelName = "gpt-4o";

            var creds = new ApiKeyCredential(configuration.GetValue<string>("AzureFoundaryProjectKey"));
            var client = new OpenAIClient(creds, options);
            kernelBuilder.AddOpenAIChatCompletion(modelName, client);

            Kernel kernel = kernelBuilder.Build();

            var a = kernel.Plugins.GetFunction("Tools", "GetProjectDetails");
            var b = kernel.Plugins.GetFunction("Tools", "GetProjectDetailsByName");

            OpenAIPromptExecutionSettings executionSettings = new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto([a,b])
            };

            // Execute a prompt using the MCP tools. The AI model will automatically call the appropriate MCP tools to answer the prompt.
            var prompt = "Show me all projects led by Alice";
            var result = await kernel.InvokePromptAsync(prompt, new(executionSettings));
            Console.WriteLine(result);
        }
    }
}

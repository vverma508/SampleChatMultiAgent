using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.AzureAI;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleChatMultiAgent
{
    public static class AzureAiAgentFactory
    {
        public static async Task GetPersistentAgentsClient()
        {
            // Add configuration support
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>() // or use your startup class
                .Build();


            PersistentAgentsClient client = AzureAIAgent.CreateAgentsClient(configuration.GetValue<string>("AzureFoundaryProjectUrl"), new AzureCliCredential());

            string modelName = "gpt-4o";
            // 1. Define an agent on the Azure AI agent service
            PersistentAgent definition = await client.Administration.CreateAgentAsync(
                modelName,
                name: "Agent 2",
                description: "This is a agent which helps finding tone of the statement",
                instructions: "For any given input find the tone of the statement");

            // 2. Create a Semantic Kernel agent based on the agent definition
            AzureAIAgent agent = new(definition, client);

            AzureAIAgentThread agentThread = new(agent.Client);
            try
            {
                ChatMessageContent message = new(AuthorRole.User, "I am very angry at you");
                await foreach (ChatMessageContent response in agent.InvokeAsync(message, agentThread))
                {
                    Console.WriteLine(response.Content);
                }
            }
            finally
            {
                await agentThread.DeleteAsync();
            }
        }
    }
}

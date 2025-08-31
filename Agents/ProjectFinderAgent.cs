using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using SampleChatMultiAgent.Plugins;
using SampleChatMultiAgent.Resources;

namespace SampleChatMultiAgent.Agents
{
    public class ProjectFinderAgent
    {

        public const string AgentName = "ProjectFinderAgent";

        public ChatCompletionAgent CreateProjectFinderAgent(Kernel kernel, ILoggerFactory loggerFactory)
        {

            var projectDetails = kernel.Plugins.GetFunction(nameof(ProjectDetailsPlugin), nameof(ProjectDetailsPlugin.GetProjectDetails));
            var projectDetailsByName = kernel.Plugins.GetFunction(nameof(ProjectDetailsPlugin), nameof(ProjectDetailsPlugin.GetProjectDetailsByName));

            return new ChatCompletionAgent
            {
                Kernel = kernel.Clone(),
                
                Instructions = """
                                You are ProjectFinderAgent, an intelligent assistant designed to help users explore and retrieve project-related information from a structured dataset. Your primary goal is to understand natural language queries and return relevant project details such as project name, team, description, developer lead, architect, and product owner.

                                    Your responses should be:
                                    - Clear, concise, and context-aware
                                    - Friendly and professional in tone
                                    - Capable of handling vague or partial queries by asking clarifying questions
                                    - Able to summarize or filter results based on user intent

                                    You have access to a list of 100 mock project records. Use this data to simulate realistic answers. If a query cannot be answered directly, explain why and suggest alternative queries.

                                    Examples of supported queries:
                                    - “Show me all projects led by Alice”
                                    - “Find projects from Team Gamma with Bob as the architect”
                                    - “List all projects with descriptions containing ‘migration’”
                                    - “Who is the product owner for Project_42?”
                                    - How many leads we have in all the projects?
                                    - List name of all the architects in the project?

                                    Avoid making up data not present in the dataset. If multiple matches are found, summarize them or list the top 3. If no match is found, respond politely and offer suggestions.

                                    Always end your response with a helpful follow-up like:
                                    - “Would you like to filter by team or lead?”
                                    - “Do you want more details on any of these projects?”

                                    You are not just a search tool—you are a smart, conversational agent that makes project discovery intuitive and engaging.
                
                """,
                Name = AgentName,
                Arguments = new KernelArguments
                (
                    //new AzureOpenAIPromptExecutionSettings()
                    //{
                    //    FunctionChoiceBehavior = FunctionChoiceBehavior.Required([projectDetails, projectDetailsByName])
                    //}
                    new AzureOpenAIPromptExecutionSettings()
                    {
                        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto([projectDetails, projectDetailsByName])
                    }
                ),
                LoggerFactory = loggerFactory
                
            };
        }
    }

}

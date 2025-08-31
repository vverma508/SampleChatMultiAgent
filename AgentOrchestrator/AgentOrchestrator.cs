using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Orchestration.Concurrent;
using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
using Microsoft.SemanticKernel.Agents.Orchestration.Sequential;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using Microsoft.SemanticKernel.ChatCompletion;
using SampleChatMultiAgent.Agents;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SampleChatMultiAgent.AgentOrchestrator
{
    public class AgentOrchestrator
    {
        [Experimental("SKEXP0110")]
        private readonly SequentialOrchestration _sequentialOrchestration;
        ChatHistory history = [];

        public List<StreamingChatMessageContent> StreamedResponses = [];
        ValueTask responseCallback(ChatMessageContent response)
        {
            history.Add(response);
            return ValueTask.CompletedTask;
        }
        protected static void WriteStreamedResponse(IEnumerable<StreamingChatMessageContent> streamedResponses)
        {
            string? authorName = null;
            AuthorRole? authorRole = null;
            StringBuilder builder = new();
            foreach (StreamingChatMessageContent response in streamedResponses)
            {
                authorName ??= response.AuthorName;
                authorRole ??= response.Role;

                if (!string.IsNullOrEmpty(response.Content))
                {
                    builder.Append($"({JsonSerializer.Serialize(response.Content)})");
                }
            }

            if (builder.Length > 0)
            {
                System.Console.WriteLine($"\n# STREAMED {authorRole ?? AuthorRole.Assistant}{(authorName is not null ? $" - {authorName}" : string.Empty)}: {builder}\n");
            }
        }

        public ValueTask StreamingResultCallback(StreamingChatMessageContent streamedResponse, bool isFinal)
        {
            this.StreamedResponses.Add(streamedResponse);

            if (isFinal)
            {
                WriteStreamedResponse(this.StreamedResponses);
                this.StreamedResponses.Clear();
            }

            return ValueTask.CompletedTask;
        }

        [Experimental("SKEXP0110")]
        public AgentOrchestrator(Kernel kernel, ILoggerFactory loggerFactory)
        {
            ProjectFinderAgent projectFinderAgent = new ProjectFinderAgent();
            var agent = projectFinderAgent.CreateProjectFinderAgent(kernel, loggerFactory);
            this._sequentialOrchestration = new SequentialOrchestration(agent)
            {
                Name = "Test Orch",
                LoggerFactory = loggerFactory,
                ResponseCallback = responseCallback,
            };

        }
        private const string ProjectFinderAgentName = ProjectFinderAgent.AgentName;

        [Experimental("SKEXP0110")]
        public async Task<string> StartAgentGroupChatAsync(string input)
        {
            // Start the runtime
            InProcessRuntime runtime = new();
            await runtime.StartAsync();

            // Invoke the orchestration and get the result
            OrchestrationResult<string> result = await _sequentialOrchestration.InvokeAsync(input, runtime);
            string text = await result.GetValueAsync(TimeSpan.FromSeconds(60));
            Console.WriteLine($"\n# RESULT: {text}"); 
            //Console.WriteLine($"\n# RESULT:\n{string.Join("\n\n", text.Select(text => $"{text}"))}");


            foreach (var item in history)
            {
                Console.WriteLine($"\n# History: {item}");
            }

            await runtime.RunUntilIdleAsync();

            return text;

        }
    }
}

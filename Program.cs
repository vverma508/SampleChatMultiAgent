// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.Orchestration.Sequential;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using SampleChatMultiAgent;
using SampleChatMultiAgent.Agents;
using SampleChatMultiAgent.Plugins;

Console.WriteLine("Hello, AI Sample Test App!");

var kernel = KernalFactory.GetKernel();
kernel.Plugins.AddFromType<ProjectDetailsPlugin>();

var loggerFactory = kernel.GetRequiredService<ILoggerFactory>();
ProjectFinderAgent projectFinderAgent = new ProjectFinderAgent();
var agent1 = projectFinderAgent.CreateProjectFinderAgent(kernel, loggerFactory);


#pragma warning disable SKEXP0130 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
SequentialOrchestration sequentialOrchestration = new SequentialOrchestration(agent1)
{
    Description="This is a sequentials orchetration",
    Name="test orch"
};

InProcessRuntime inProcessRuntime = new InProcessRuntime();
await inProcessRuntime.StartAsync();

var res = await sequentialOrchestration.InvokeAsync("Show me all projects led by Alice", inProcessRuntime);

var finalResult = await res.GetValueAsync(TimeSpan.FromSeconds(20));
Console.WriteLine("Final Result:");
Console.WriteLine(finalResult);

await inProcessRuntime.RunUntilIdleAsync();


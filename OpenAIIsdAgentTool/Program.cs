using Azure.AI.OpenAI;
using Azure.Core;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAIIsdAgentTool;
using System.Text.Json;

// Demonstrates using raw OpenAI IChatClient, creating an agent with tool support registering ISDAgent as a tool at creation time.
// Environment variables required:
// OPENAI_ENDPOINT (optional - if using non default)
// OPENAI_MODEL (e.g. gpt-4o-mini, gpt-4.1-mini, etc.)
// OPENAI_API_KEY

Console.Clear();
Console.WriteLine("ISD Agent Tool Sample\n");

// Local ISDAgent instance
var isdAgent = new ISDAgent();

var instructions = "You are a helpful assistant.";

var chatClient = CreateAzureChatClient();

AIAgent chatAgent = chatClient.CreateAIAgent(instructions: instructions, tools: [isdAgent.AsAIFunction()]);

Console.WriteLine("Enter a prompt (blank to exit). Try: 'hello ISD'");
var thread = chatAgent.GetNewThread();

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) break;
    var response = await chatAgent.RunAsync(input, thread);
    var messages = thread.GetService<IList<ChatMessage>>();
    Console.WriteLine(response.ToString());
    Console.WriteLine();
    Console.WriteLine(JsonSerializer.Serialize(messages, new JsonSerializerOptions { WriteIndented = true }));
    Console.WriteLine();
}

IChatClient CreateAzureChatClient()
{
    var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
    var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o-mini";
    var authorityHostEnv = Environment.GetEnvironmentVariable("AZURE_OPENAI_AUTHORITY_HOST");
    AzureOpenAIClientOptions? azureOptions = null;
    TokenCredential credential;
    if (!string.IsNullOrWhiteSpace(authorityHostEnv))
    {
        azureOptions = new AzureOpenAIClientOptions { Audience = AzureOpenAIAudience.AzureGovernment };
        credential = new AzureCliCredential(new AzureCliCredentialOptions { AuthorityHost = new Uri(authorityHostEnv) });
    }
    else
    {
        credential = new AzureCliCredential();
    }
    return new AzureOpenAIClient(new Uri(endpoint), credential, options: azureOptions)
        .GetChatClient(deploymentName)
        .AsIChatClient();
}

using Azure.AI.OpenAI;
using Azure.Core;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

// This sample demonstrates a multi-turn conversation reusing the same agent (and underlying thread/state) between prompts.
// Environment variables required:
// AZURE_OPENAI_ENDPOINT (e.g. https://my-openai-resource.openai.azure.com/)
// AZURE_OPENAI_DEPLOYMENT_NAME (e.g. gpt-4o-mini)
// Optional:
// AZURE_OPENAI_AUTHORITY_HOST (for Azure Gov, e.g. https://login.microsoftonline.us/)

Console.Clear();
Console.WriteLine("Multi-Turn Conversation Sample\n");

var systemInstructions = "You are a helpful assistant that gives concise answers.";
var firstUserPrompt = "List three core qualities of a great software consultant.";
var followUpPrompt = "Expand briefly on the second quality only.";

// Create chat client (Azure OpenAI only)
var chatClient = CreateAzureChatClient();

// Create agent (stateful across RunAsync calls)
AIAgent agent = chatClient.CreateAIAgent(instructions: systemInstructions, name: "ISDShotsAgent-MultiTurn");

// Create new thread for conversation
AgentThread thread = agent.GetNewThread();

// First turn
Console.WriteLine($"> User: {firstUserPrompt}\n");
var firstResponse = await agent.RunAsync(firstUserPrompt, thread);
Console.WriteLine("Assistant (first response):");
Console.WriteLine(firstResponse.ToString());
Console.WriteLine();

// Second turn using same agent (framework will append to conversation state internally)
Console.WriteLine($"> User (follow-up): {followUpPrompt}\n");
var followUpResponse = await agent.RunAsync(followUpPrompt, thread);
Console.WriteLine("Assistant (follow-up response):");
Console.WriteLine(followUpResponse.ToString());
Console.WriteLine();

// Local helper to create Azure OpenAI IChatClient
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
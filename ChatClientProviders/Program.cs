using Azure.AI.OpenAI;
using Azure.Core; // TokenCredential
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel; // For ApiKeyCredential

// Shared instructions
var instructions = "You are a helpful assistant.";
var prompt = "In few words What makes a great consultant?";

Console.Clear();
Console.WriteLine(prompt);

// Execute for Azure OpenAI
var azureChatClient = CreateAzureChatClient();
Console.WriteLine("Azure OpenAI response:");
Console.WriteLine(await RunPromptAsync(azureChatClient, instructions, prompt, "ISDShotsAgent-Azure"));

// Execute for Ollama
var ollamaChatClient = CreateOllamaChatClient();
Console.WriteLine();
Console.WriteLine("Ollama response:");
Console.WriteLine(await RunPromptAsync(ollamaChatClient, instructions, prompt, "ISDShotsAgent-Ollama"));

// Local function to create Azure OpenAI IChatClient
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

// Local function to create Ollama IChatClient
IChatClient CreateOllamaChatClient()
{
    var ollamaEndpoint = Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434/v1"; // default local endpoint
    var ollamaModel = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "llama3.2";
    var ollamaApiKey = Environment.GetEnvironmentVariable("OLLAMA_API_KEY") ?? "ollama"; // placeholder key
    var ollamaClientOptions = new OpenAIClientOptions { Endpoint = new Uri(ollamaEndpoint) };
    return new OpenAIClient(new ApiKeyCredential(ollamaApiKey), ollamaClientOptions)
        .GetChatClient(ollamaModel)
        .AsIChatClient();
}

// Local function to run an agent prompt avoiding repetition
async Task<string> RunPromptAsync(IChatClient chatClient, string agentInstructions, string agentPrompt, string agentName)
{
    AIAgent agent = chatClient.CreateAIAgent(instructions: agentInstructions, name: agentName);
    var response = await agent.RunAsync(agentPrompt);
    return response.ToString();
}



# Microsoft Agent Framework Chat Client Providers Sample

Demonstrates creating `AIAgent` instances with the Microsoft Agent Framework targeting both Azure OpenAI and an Ollama (OpenAI-compatible) endpoint using only environment variables. Shows simple invocation producing a hello ISD SHOTS sample JSON.

## Environment Variables

Required (Azure OpenAI):
- `AZURE_OPENAI_ENDPOINT` : Azure OpenAI endpoint URL (e.g. `https://my-openai-resource.openai.azure.com/`)
- `AZURE_OPENAI_DEPLOYMENT_NAME` : Deployment name (e.g. `gpt-4o-mini`)

Optional (Azure Government / AAD authority):
- `AZURE_OPENAI_AUTHORITY_HOST` : Authority host (e.g. `https://login.microsoftonline.us/`) to set credential authority and switch audience to Azure Government.

Required (Ollama - if you want Ollama output):
- `OLLAMA_ENDPOINT` : Ollama OpenAI-compatible base URL (default used by sample if unset: `http://localhost:11434/v1`)
- `OLLAMA_MODEL` : Ollama model name (default used by sample if unset: `llama3.2`)

Optional (Ollama):
- `OLLAMA_API_KEY` : API key if your Ollama endpoint enforces one (sample supplies a placeholder when unset).

## PowerShell Setup & Run
```powershell
# Azure OpenAI
$Env:AZURE_OPENAI_ENDPOINT = "https://my-openai-resource.openai.azure.com/"
$Env:AZURE_OPENAI_DEPLOYMENT_NAME = "gpt-4o-mini"
# (Optional Azure Gov authority host)
# $Env:AZURE_OPENAI_AUTHORITY_HOST = "https://login.microsoftonline.us/"

# Ollama (adjust if different model or remote host)
$Env:OLLAMA_ENDPOINT = "http://localhost:11434/v1"
$Env:OLLAMA_MODEL = "llama3.2"
# (Optional key if your Ollama server requires it)
# $Env:OLLAMA_API_KEY = "your-key"

# Authenticate for Azure (ensure prior login)
az login

# Run sample
DOTNET_NOLOGO=1 dotnet run --project ChatClientProviders
```

## What the Program Does
1. Creates an Azure OpenAI `IChatClient` (configuring AzureGov audience if `AZURE_OPENAI_AUTHORITY_HOST` is set).
2. Creates an Ollama `IChatClient` via OpenAI-compatible API endpoint.
3. For each chat client, creates an `AIAgent` with shared instructions and runs a single prompt, printing JSON-like output.

## Simplified Code Flow
```csharp
var azureChatClient = CreateAzureChatClient();
Console.WriteLine(await RunPromptAsync(azureChatClient, instructions, prompt, "ISDShotsAgent-Azure"));

var ollamaChatClient = CreateOllamaChatClient();
Console.WriteLine(await RunPromptAsync(ollamaChatClient, instructions, prompt, "ISDShotsAgent-Ollama"));
```

## Expected JSON Output (Representative)
```json
{
 "greeting": "Hello ISD SHOTS!"
}
```

## Notes
- Uses `AzureCliCredential`; you can swap for Managed Identity or other `TokenCredential`.
- Authority host triggers Azure Government audience automatically in the sample.
- Ollama settings fall back to defaults if not provided; unset variables still allow Azure path.
- All configuration comes from environment variables; no extra configuration files required.

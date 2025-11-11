using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace SimpleISDAgent
{
    public class ISDAgent : AIAgent
    {
        public ISDAgent() { }
        public override AgentThread GetNewThread() => new SimpleInMemoryAgentThread();
        public override AgentThread DeserializeThread(JsonElement serializedThread, JsonSerializerOptions? jsonSerializerOptions = null)
        => new SimpleInMemoryAgentThread(serializedThread, jsonSerializerOptions);
        public override Task<AgentRunResponse> RunAsync(IEnumerable<ChatMessage> messages, AgentThread? thread = null, AgentRunOptions? options = null, CancellationToken cancellationToken = default)
        {
            var lastUser = messages.LastOrDefault(m => m.Role == ChatRole.User);
            var userText = lastUser?.ToString() ?? string.Empty;
            var assistantMessage = new ChatMessage(ChatRole.Assistant, userText.ToUpperInvariant());
            return Task.FromResult(new AgentRunResponse(new List<ChatMessage> { assistantMessage }));
        }
        public override async IAsyncEnumerable<AgentRunResponseUpdate> RunStreamingAsync(IEnumerable<ChatMessage> messages, AgentThread? thread = null, AgentRunOptions? options = null, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var lastUser = messages.LastOrDefault(m => m.Role == ChatRole.User);
            var userText = lastUser?.ToString() ?? string.Empty;
            var assistantMessage = new ChatMessage(ChatRole.Assistant, userText.ToUpperInvariant());
            var response = new AgentRunResponse(new List<ChatMessage> { assistantMessage });
            foreach (var update in response.ToAgentRunResponseUpdates())
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return update;
            }
        }
    }
    public class SimpleInMemoryAgentThread : InMemoryAgentThread
    {
        public SimpleInMemoryAgentThread() { }
        public SimpleInMemoryAgentThread(JsonElement serializedThreadState, JsonSerializerOptions? jsonSerializerOptions = null)
        : base(serializedThreadState, jsonSerializerOptions) { }
    }
}

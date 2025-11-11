using Microsoft.Extensions.AI;
using SimpleISDAgent;

Console.Clear();
Console.WriteLine("Simple ISD Agent Uppercase Sample (AgentRunResponse)\n");
Console.WriteLine("Enter text (blank line to exit)\n");

var agent = new ISDAgent();
var thread = agent.GetNewThread();

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input))
    {
        break;
    }
    var userMessage = new ChatMessage(ChatRole.User, input);
    var response = await agent.RunAsync(new[] { userMessage }, thread);
    Console.WriteLine(response.ToString());
}

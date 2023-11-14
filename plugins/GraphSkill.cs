using System.Net.Http.Headers;
using System.ComponentModel;
using System.Net.Http;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Plugins;
using Microsoft.SemanticKernel.Plugins.Core;

public class GraphSkill
{

    private readonly string key;
    public GraphSkill(string key) => this.key = key;
   
    [SKFunction, Description("Get the user's information from Microsoft Graph")]
    public async Task<string> GetUserAsync(
        [Description("The user information from MS Graph")] string username
    )
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("User-Agent", "C# app");   

        var url = $"https://graph.microsoft.com/v1.0/users/{username}";

        var json = await client.GetStringAsync(url);
        
        return json;
    }
}
using System.ComponentModel;
using System.Net;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.SemanticKernel;

namespace Plugins.GraphPlugins
{
    public class MsGraphPlugin
    {   
        [SKFunction, Description("Get the user's information from Microsoft Graph")]
        [Function("GetUser")]
        public static async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req, 
            string username,
            // string meetingId,
            // string transcriptId,
            FunctionContext executionContext)
        {
            var logger =  executionContext.GetLogger("GetUser");
            logger.LogInformation($"GetUser is called. Username:{username}");


            var scopes = new string[] { "https://graph.microsoft.com/.default" };

            var options = new ClientSecretCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };
            var clientSecretCredential = new ClientSecretCredential(
                Constants.TenantId,
                Constants.AppId,
                Constants.ClientSecret,
                options
            );
            
            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

            var user = await graphClient
            .Users
            .GetAsync((requestConfig) => {
                requestConfig.QueryParameters.Select = new string[] { "id" };
                requestConfig.QueryParameters.Filter = $"userPrincipalName eq '{username}'";
            });

            var userId = user?.Value?[0]?.Id;

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString($"User: {user?.Value?[0]?.Id}");

            return response;
        }

    }
}

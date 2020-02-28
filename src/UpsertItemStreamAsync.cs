using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Mobsites.Azure.Functions.Cosmos.Extension;
using Azure.Cosmos;
using System.Net;

namespace Mobsites.Azure.Functions.Cosmos.Stream.API
{
    public static class UpsertItemStreamAsync
    {
        [FunctionName("UpsertItemStreamAsync")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "cosmos/{database}/{container}/{partitionKey}")] HttpRequest req,
            [Cosmos(
                databaseName: "{database}",
                containerName: "{container}",
                ConnectionStringSetting = "Cosmos")] CosmosContainer cosmosContainer,
            string partitionKey)
        {
            var response = await cosmosContainer.UpsertItemStreamAsync(req.Body, new PartitionKey(partitionKey));

            return (HttpStatusCode)response.Status == HttpStatusCode.OK
                ? new FileStreamResult(response.ContentStream, "application/json")
                : (IActionResult)new StatusCodeResult(response.Status);
        }
    }
}

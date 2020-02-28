using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Mobsites.Azure.Functions.Cosmos.Extension;
using Azure.Cosmos;

namespace Mobsites.Azure.Functions.Cosmos.Stream.API
{
    public static class DeleteItemStreamAsync
    {
        [FunctionName("DeleteItemStreamAsync")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "cosmos/{database}/{container}/{partitionKey}/{id}")] HttpRequest req,
            [Cosmos(
                databaseName: "{database}",
                containerName: "{container}",
                ConnectionStringSetting = "Cosmos")] CosmosContainer cosmosContainer,
            string partitionKey,
            string id)
        {
            var response = await cosmosContainer.DeleteItemStreamAsync(id, new PartitionKey(partitionKey));

            return new StatusCodeResult(response.Status);
        }
    }
}

// Copyright (c) 2020 Allan Mobley. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

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
    public static class CreateItemStreamAsync
    {
        [FunctionName("CreateItemStreamAsync")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "cosmos/{database}/{container}/{partitionKey}")] HttpRequest req,
            [Cosmos(
                databaseName: "{database}",
                containerName: "{container}",
                ConnectionStringSetting = "Cosmos")] CosmosContainer cosmosContainer,
            string partitionKey)
        {
            var response = await cosmosContainer.CreateItemStreamAsync(req.Body, new PartitionKey(partitionKey));

            return (HttpStatusCode)response.Status == HttpStatusCode.Created
                ? new FileStreamResult(response.ContentStream, "application/json")
                : (IActionResult)new StatusCodeResult(response.Status);
        }
    }
}
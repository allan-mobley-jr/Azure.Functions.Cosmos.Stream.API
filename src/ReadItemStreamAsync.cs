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
    public static class ReadItemStreamAsync
    {
        [FunctionName("ReadItemStreamAsync")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cosmos/{database}/{container}/{partitionKey}/{id}")] HttpRequest req,
            [Cosmos(
                databaseName: "{database}",
                containerName: "{container}",
                ConnectionStringSetting = "Cosmos")] CosmosContainer cosmosContainer,
            string partitionKey,
            string id)
        {
            var response = await cosmosContainer.ReadItemStreamAsync(id, new PartitionKey(partitionKey));

            return (HttpStatusCode)response.Status == HttpStatusCode.OK
                ? new FileStreamResult(response.ContentStream, "application/json")
                : (IActionResult)new StatusCodeResult(response.Status);
        }
    }
}
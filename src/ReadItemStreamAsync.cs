// Copyright (c) 2020 Allan Mobley. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Azure.Cosmos;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Mobsites.Azure.Functions.Cosmos.Extension;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mobsites.Azure.Functions.Cosmos.Stream.API
{
    public static class ReadItemStreamAsync
    {
        [FunctionName("ReadItemStreamAsync")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cosmos/{database}/{container}/{partitionKey}/{id}")] HttpRequest req,
            [Cosmos(
                databaseName: "{database}",
                containerName: "{container}",
                ConnectionStringSetting = "Cosmos")] CosmosContainer cosmosContainer,
            string partitionKey,
            string id)
        {
            var response = await cosmosContainer.ReadItemStreamAsync(id, new PartitionKey(partitionKey));

            // Would be really nice if we could simply pass the Cosmos response directly back,
            // but, alas, we must reconstruct a return response.
            var result = new HttpResponseMessage();

            // Pass Cosmos response headers back.
            foreach (var headerName in response.Headers)
            {
                result.Headers.Add(headerName.Name, headerName.Value);
            }

            // Pass Cosmos status code back.
            result.StatusCode = (HttpStatusCode)response.Status;

            if (response.ContentStream != null)
                // Pass stream directly to response object, without deserializing.
                result.Content = new StreamContent(response.ContentStream);

            return result;
        }
    }
}
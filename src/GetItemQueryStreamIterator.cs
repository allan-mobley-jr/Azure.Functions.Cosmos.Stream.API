// Copyright (c) 2020 Allan Mobley. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Azure.Cosmos;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Mobsites.Azure.Functions.Cosmos.Extension;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace Mobsites.Azure.Functions.Cosmos.Stream.API
{
    public static class GetItemQueryStreamIterator
    {
        [FunctionName("GetItemQueryStreamIterator")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "cosmos/{database}/{container}/{partitionKey}/items/{maxItemCount?}")] HttpRequest req,
            [Cosmos(
                databaseName: "{database}",
                containerName: "{container}",
                ConnectionStringSetting = "Cosmos")] CosmosContainer cosmosContainer,
            string partitionKey,
            int? maxItemCount)
        {
            // Would be really nice if we could simply pass a Cosmos response directly back,
            // but, alas, we must reconstruct a return response.
            var result = new HttpResponseMessage();

            // Guard against incorrect request body.
            try
            {
                // Get QuerySpec object from request body.
                var querySpec = await JsonSerializer.DeserializeAsync<QuerySpec>(req.Body, Serialization.Options);

                var queryDefinition = new QueryDefinition(querySpec.Query);

                if (querySpec.Parameters.Length > 0)
                {
                    foreach (var parameter in querySpec.Parameters)
                    {
                        queryDefinition = queryDefinition.WithParameter(parameter.Name, parameter.Value);
                    }
                }

                string continuationToken = string.IsNullOrWhiteSpace(req.Headers["Continuation-Token"])
                    ? null
                    : (string)req.Headers["Continuation-Token"];

                var responses = cosmosContainer.GetItemQueryStreamIterator(
                    queryDefinition,
                    continuationToken,
                    new QueryRequestOptions
                    {
                        PartitionKey = new PartitionKey(partitionKey),
                        MaxItemCount = maxItemCount
                    });

                await foreach (var response in responses)
                {
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
                    
                    break;
                }
            }
            catch (Exception ex)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Content = new StringContent(ex.Message);
            }

            return result;
        }
    }
}
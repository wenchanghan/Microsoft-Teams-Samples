namespace GraphConnectorsIntegration.Controllers
{
    using GraphConnectorsIntegration.Services.GraphService;
    using GraphConnectorsIntegration.Services.GraphService.Models;
    using GraphConnectorsIntegration.Services.GraphService.Models.ExternalItems;
    using GraphConnectorsIntegration.Utilities;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Graph;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    [ApiController]
    [Route("[controller]")]
    public class GraphConnectorsController : ControllerBase
    {
        private readonly WebhookTokenValidator webhookTokenValidator;
        private readonly IGraphService graphService;

        public GraphConnectorsController(IGraphService graphService)
        {
            this.webhookTokenValidator = new WebhookTokenValidator();
            this.graphService = graphService ?? throw new ArgumentNullException(nameof(graphService));
        }

        [HttpPost]
        [Route("ProcessGraphWebhookChangeNotificationCollection")] // This API route should match the Graph Connectors notification url in Teams app's manifest.
        public async Task<IActionResult> ProcessGraphWebhookChangeNotificationCollectionAsync([FromBody] ChangeNotificationCollection changeNotificationCollection)
        {
            // The suggested way to handle Graph Webhook change notification is to leverage Web-Queue-Worker architecture. Queue the notification and immediately return an HTTP2xx.
            // This sample app processes the entire change notification within the API call, purely for simplicity.

            if (changeNotificationCollection?.Value?.Count() != 1
                || changeNotificationCollection.ValidationTokens?.Count() != 1)
            {
                // Invalid payload. Discard.
                return this.Ok();
            }

            string validationToken = changeNotificationCollection.ValidationTokens.Single();
            ChangeNotification changeNotification = changeNotificationCollection.Value.Single();
            string tenantIdFromNotification = changeNotification.TenantId?.ToString();

            try
            {
                await this.webhookTokenValidator.ValidateToken(validationToken, tenantIdFromNotification, ServiceConstants.AppId, new CancellationToken());
            }
            catch
            {
                // Notification not from trusted source. Discard.
                return this.Ok();
            }

            IDictionary<string, object> changeDetails = changeNotification.ResourceData?.AdditionalData;
            string targetConnectorState = GetChangeDetailByName(changeDetails, "state");

            if (!changeNotification.Resource.Contains("external"))
            {
                // Resource not in scope for processing. Discard.
                return this.Ok();
            }

            // Validation completed. Process Graph Connector change.
            // Ideally, this should be done in a worker/processor after ack'ing the Webhook notification call.
            if (string.Equals("disabled", targetConnectorState, StringComparison.OrdinalIgnoreCase))
            {
                ODataCollection<ExternalConnection> externalConnections = await this.graphService.GetExternalConnectionsAsync(tenantIdFromNotification);

                // Remove all connections for the current app. Existing data in connections will be deleted too.
                // For simplicity, firing deletions in parallel. Please add robust concurrency management (i.e. Semaphore) for production scenarios.
                if (externalConnections?.Value.Any() == true)
                {
                    await Task.WhenAll(externalConnections.Value.Select(c => this.graphService.DeleteExternalConnectionAsync(tenantIdFromNotification, c.Id)));
                }
            }
            else if (string.Equals("enabled", targetConnectorState, StringComparison.OrdinalIgnoreCase))
            {
                string connectorId = GetChangeDetailByName(changeDetails, "id");
                string connectorTicket = GetChangeDetailByName(changeDetails, "connectorsTicket");

                // For simplicity, creating a single connection.
                ExternalConnection newConnection = new ExternalConnection
                {
                    Id = "contosohr",
                    Name = "Contoso HR",
                    Description = "Connection to index Contoso HR system",
                    ConnectorId = connectorId,
                    EnabledContentExperiences = "MicrosoftSearch, Compliance, IntelligentDiscovery",
                };
                ExternalConnection createdConnection = await this.graphService.PostExternalConnectionAsync(tenantIdFromNotification, newConnection, connectorTicket);

                Schema schemaForNewConnection = new Schema
                {
                    Properties = new List<SchemaProperty>
                    {
                        new SchemaProperty { Name = GetJsonPropertyName(typeof(ContosoHrExternalItem), nameof(ContosoHrExternalItem.Title)), Type = "String", IsSearchable = true, IsRetrievable = true, Labels = new string[] { "title" } },
                        new SchemaProperty { Name = GetJsonPropertyName(typeof(ContosoHrExternalItem), nameof(ContosoHrExternalItem.Priority)), Type = "String", IsQueryable = true, IsRetrievable = true, IsSearchable = false },
                        new SchemaProperty { Name = GetJsonPropertyName(typeof(ContosoHrExternalItem), nameof(ContosoHrExternalItem.Assignee)), Type = "String", IsRetrievable = true },
                    }
                };
                await this.graphService.PostExternalConnectionSchemaAsync(tenantIdFromNotification, createdConnection.Id, schemaForNewConnection);
            }
            else
            {
                // Invalid target state. Discard.
                return this.Ok();
            }

            // Placeholder - Persist customer's connection state for data ingestion flow.
            return this.Ok();
        }

        [HttpPost]
        [Route("SimulateDataIngestion")] // This is only to demonstrate how to integrate with Graph Connectors' data ingestion API. The logics should lie inside of app's existing business logics.
        public async Task<IActionResult> SimulateDataIngestionAsync([FromQuery] string customerTenantId, [FromQuery] string connectionId)
        {
            if (string.IsNullOrWhiteSpace(customerTenantId))
            {
                throw new ArgumentNullException(nameof(customerTenantId));
            }

            if (string.IsNullOrWhiteSpace(connectionId))
            {
                throw new ArgumentNullException(nameof(connectionId));
            }

            ExternalConnection externalConnection = await this.graphService.GetExternalConnectionByIdAsync(customerTenantId, connectionId);
            if (!string.Equals(externalConnection?.State, "ready", StringComparison.OrdinalIgnoreCase))
            {
                return this.StatusCode((int)HttpStatusCode.PreconditionFailed);
            }

            // This is just a sample app data.
            string itemId = "TSP228082938";
            ExternalItem<ContosoHrExternalItem> externalItem = new ExternalItem<ContosoHrExternalItem>
            {
                Acl = new Acl[]
                {
                    new Acl { Type = AclType.User, Value = "e811976d-83df-4cbd-8b9b-5215b18aa874", AccessType = AclAccessType.Grant },
                    new Acl { Type = AclType.ExternalGroup, Value = "14m1b9c38qe647f6a", AccessType = AclAccessType.Deny }
                },
                Properties = new ContosoHrExternalItem
                {
                    Title = "Error in the payment gateway",
                    Priority = 1,
                    Assignee = "john@contoso.com"
                },
                Content = new ExternalItemContent
                {
                    Value = "Error in payment gateway...",
                    Type = "text"
                }
            };
            await this.graphService.PutExternalItemAsync(customerTenantId, connectionId, itemId, externalItem);
            return this.Ok();
        }

        private static string GetChangeDetailByName(IDictionary<string, object> changeDetails, string key)
        {
            object detailObject = null;
            changeDetails?.TryGetValue(key, out detailObject);
            return detailObject?.ToString();
        }

        private static string GetJsonPropertyName(Type type, string propertyName)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetProperty(propertyName)
                ?.GetCustomAttribute<JsonPropertyAttribute>()
                ?.PropertyName;
        }
    }
}

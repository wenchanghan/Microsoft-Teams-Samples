namespace GraphConnectorsIntegration.Controllers
{
    using GraphConnectorsIntegration.Services.GraphService;
    using GraphConnectorsIntegration.Services.GraphService.Models;
    using GraphConnectorsIntegration.Utilities;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Graph;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [ApiController]
    [Route("[controller]")]
    public class GraphConnectorsController : ControllerBase
    {
        private const string AppId = "";
        private readonly WebhookTokenValidator webhookTokenValidator;
        private readonly IGraphService graphService;

        public GraphConnectorsController(IGraphService graphService)
        {
            this.webhookTokenValidator = new WebhookTokenValidator();
            this.graphService = graphService ?? throw new ArgumentNullException(nameof(graphService));
        }

        [HttpPost]
        [Route("ProcessGraphWebhookChangeNotificationCollection")]
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
                await this.webhookTokenValidator.ValidateToken(validationToken, tenantIdFromNotification, AppId, new CancellationToken());
            }
            catch
            {
                // Notification not from trusted source. Discard.
                return this.Ok();
            }

            IDictionary<string, object> changeDetails = changeNotification.ResourceData?.AdditionalData;
            string targetConnectorState = GetChangeDetailByName(changeDetails, "state");

            if (!changeNotification.Resource.Contains("external/connectors"))
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
                        new SchemaProperty { Name = "ticketTitle", Type = "String" },
                        new SchemaProperty { Name = "priority", Type = "String" },
                        new SchemaProperty { Name = "assignee", Type = "String" },
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

        private static string GetChangeDetailByName(IDictionary<string, object> changeDetails, string key)
        {
            object detailObject = null;
            changeDetails?.TryGetValue(key, out detailObject);
            return detailObject?.ToString();
        }
    }
}

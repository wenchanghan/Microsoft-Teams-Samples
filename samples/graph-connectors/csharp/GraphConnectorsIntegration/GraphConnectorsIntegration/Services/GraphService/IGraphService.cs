namespace GraphConnectorsIntegration.Services.GraphService
{
    using GraphConnectorsIntegration.Services.GraphService.Models;
    using System.Threading.Tasks;

    public interface IGraphService
    {
        public Task<ODataCollection<ExternalConnection>> GetExternalConnectionsAsync(string tenantId);

        public Task<ExternalConnection> PostExternalConnectionAsync(string tenantId, ExternalConnection externalConnection, string connectorTicket);

        public Task DeleteExternalConnectionAsync(string tenantId, string connectionId);

        public Task<Schema> PostExternalConnectionSchemaAsync(string tenantId, string connectionId, Schema schema);
    }
}

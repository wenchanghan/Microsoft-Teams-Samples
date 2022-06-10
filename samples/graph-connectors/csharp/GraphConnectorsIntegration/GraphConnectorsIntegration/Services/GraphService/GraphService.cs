namespace GraphConnectorsIntegration.Services.GraphService
{
    using GraphConnectorsIntegration.Services.AadService;
    using GraphConnectorsIntegration.Services.GraphService.Models;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class GraphService : IGraphService
    {
        private const string BaseUrl = "https://graph.microsoft.com";
        private const string ResourceId = "https://graph.microsoft.com";
        private readonly HttpClient httpClient;
        private readonly IAadService aadService;

        public GraphService(HttpClient httpClient, IAadService aadService)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.aadService = aadService ?? throw new ArgumentNullException(nameof(aadService));
        }

        public async Task<ODataCollection<ExternalConnection>> GetExternalConnectionsAsync(string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new ArgumentNullException(nameof(tenantId));
            }

            string url = $"{BaseUrl}/beta/external/connections";
            string token = await this.aadService.GetAccessTokenForAppAsync(tenantId, ResourceId);
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.Add("Authorization", $"Bearer {token}");
                HttpResponseMessage response = await this.httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ODataCollection<ExternalConnection>>(responseBody);
            }
        }

        public async Task<ExternalConnection> GetExternalConnectionByIdAsync(string tenantId, string connectionId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new ArgumentNullException(nameof(tenantId));
            }

            if (string.IsNullOrWhiteSpace(connectionId))
            {
                throw new ArgumentNullException(nameof(connectionId));
            }

            string url = $"{BaseUrl}/beta/external/connections/{connectionId}";
            string token = await this.aadService.GetAccessTokenForAppAsync(tenantId, ResourceId);
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.Add("Authorization", $"Bearer {token}");
                HttpResponseMessage response = await this.httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ExternalConnection>(responseBody);
            }
        }

        public async Task<ExternalConnection> PostExternalConnectionAsync(string tenantId, ExternalConnection externalConnection, string connectorTicket)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new ArgumentNullException(nameof(tenantId));
            }

            if (externalConnection == null)
            {
                throw new ArgumentNullException(nameof(externalConnection));
            }

            if (string.IsNullOrWhiteSpace(connectorTicket))
            {
                throw new ArgumentNullException(nameof(connectorTicket));
            }

            string url = $"{BaseUrl}/beta/external/connections";
            string token = await this.aadService.GetAccessTokenForAppAsync(tenantId, ResourceId);
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(externalConnection), Encoding.UTF8, "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");
                request.Headers.Add("GraphConnectors-Ticket", connectorTicket);
                HttpResponseMessage response = await this.httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ExternalConnection>(responseBody);
            }
        }

        public async Task DeleteExternalConnectionAsync(string tenantId, string connectionId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new ArgumentNullException(nameof(tenantId));
            }

            if (string.IsNullOrWhiteSpace(connectionId))
            {
                throw new ArgumentNullException(nameof(connectionId));
            }

            string url = $"{BaseUrl}/beta/external/connections/{connectionId}";
            string token = await this.aadService.GetAccessTokenForAppAsync(tenantId, ResourceId);
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url))
            {
                request.Headers.Add("Authorization", $"Bearer {token}");
                HttpResponseMessage response = await this.httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return;
            }
        }

        public async Task<Schema> PostExternalConnectionSchemaAsync(string tenantId, string connectionId, Schema schema)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new ArgumentNullException(nameof(tenantId));
            }

            if (string.IsNullOrWhiteSpace(connectionId))
            {
                throw new ArgumentNullException(nameof(connectionId));
            }

            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            string url = $"{BaseUrl}/beta/external/connections/{connectionId}/schema";
            string token = await this.aadService.GetAccessTokenForAppAsync(tenantId, ResourceId);
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(schema), Encoding.UTF8, "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");
                HttpResponseMessage response = await this.httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Schema>(responseBody);
            }
        }

        public async Task PutExternalItemAsync<ExternalItem>(string tenantId, string connectionId, string itemId, ExternalItem externalItem)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new ArgumentNullException(nameof(tenantId));
            }

            if (string.IsNullOrWhiteSpace(connectionId))
            {
                throw new ArgumentNullException(nameof(connectionId));
            }

            if (string.IsNullOrWhiteSpace(itemId))
            {
                throw new ArgumentNullException(nameof(itemId));
            }

            if (externalItem == null)
            {
                throw new ArgumentNullException(nameof(externalItem));
            }

            string url = $"{BaseUrl}/beta/external/connections/{connectionId}/items/{itemId}";
            string token = await this.aadService.GetAccessTokenForAppAsync(tenantId, ResourceId);
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, url))
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(externalItem), Encoding.UTF8, "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");
                HttpResponseMessage response = await this.httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task<ExternalItem<T>> GetExternalItemAsync<T>(string tenantId, string connectionId, string itemId) where T: ExternalItemProperty
        {
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new ArgumentNullException(nameof(tenantId));
            }

            if (string.IsNullOrWhiteSpace(connectionId))
            {
                throw new ArgumentNullException(nameof(connectionId));
            }

            if (string.IsNullOrWhiteSpace(itemId))
            {
                throw new ArgumentNullException(nameof(itemId));
            }

            string url = $"{BaseUrl}/beta/external/connections/{connectionId}/items/{itemId}";
            string token = await this.aadService.GetAccessTokenForAppAsync(tenantId, ResourceId);
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.Add("Authorization", $"Bearer {token}");
                HttpResponseMessage response = await this.httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ExternalItem<T>>(responseBody);
            }
        }
    }
}

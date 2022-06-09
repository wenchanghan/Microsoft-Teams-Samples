namespace GraphConnectorsIntegration.Services.GraphService.Models
{
    using Newtonsoft.Json;

    public class ExternalConnection
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "connectorId")]
        public string ConnectorId { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "enabledContentExperiences")]
        public string EnabledContentExperiences { get; set; }
    }
}

namespace GraphConnectorsIntegration.Services.GraphService.Models
{
    using Newtonsoft.Json;

    public class ExternalItemContent
    {
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}

namespace GraphConnectorsIntegration.Services.GraphService.Models
{
    using Newtonsoft.Json;

    public class Acl
    {
        [JsonProperty(PropertyName = "type")]
        public AclType Type { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "accessType")]
        public AclAccessType AccessType { get; set; }
    }
}

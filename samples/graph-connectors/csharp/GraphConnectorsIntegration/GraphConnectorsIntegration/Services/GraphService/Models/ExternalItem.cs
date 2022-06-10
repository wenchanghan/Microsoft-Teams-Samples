namespace GraphConnectorsIntegration.Services.GraphService.Models
{
    using Newtonsoft.Json;

    public class ExternalItem<T> where T : ExternalItemProperty
    {
        [JsonProperty(PropertyName = "acl")]
        public Acl[] Acl { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public T Properties { get; set; } 

        [JsonProperty(PropertyName = "content")]
        public ExternalItemContent Content { get; set; }
    }
}

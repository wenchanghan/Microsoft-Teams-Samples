namespace GraphConnectorsIntegration.Services.GraphService.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class Schema
    {
        [JsonProperty(PropertyName = "baseType")]
        public string BaseType { get; } = "microsoft.graph.externalItem";

        [JsonProperty(PropertyName = "properties")]
        public IEnumerable<SchemaProperty> Properties { get; set; }
    }
}

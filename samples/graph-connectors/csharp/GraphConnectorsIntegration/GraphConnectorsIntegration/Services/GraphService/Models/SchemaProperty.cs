namespace GraphConnectorsIntegration.Services.GraphService.Models
{
    using Newtonsoft.Json;

    public class SchemaProperty
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "isQueryable")]
        public bool IsQueryable { get; set; }

        [JsonProperty(PropertyName = "isRefinable")]
        public bool IsRefinable { get; set; }

        [JsonProperty(PropertyName = "isRetrievable")]
        public bool IsRetrievable { get; set; }

        [JsonProperty(PropertyName = "isSearchable")]
        public bool IsSearchable { get; set; }

        [JsonProperty(PropertyName = "labels")]
        public string[] Labels { get; set; }
    }
}

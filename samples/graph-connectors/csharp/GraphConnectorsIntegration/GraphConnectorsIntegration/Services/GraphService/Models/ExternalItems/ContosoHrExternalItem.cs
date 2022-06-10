namespace GraphConnectorsIntegration.Services.GraphService.Models.ExternalItems
{
    using Newtonsoft.Json;

    public class ContosoHrExternalItem : ExternalItemProperty
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "priority")]
        public int Priority { get; set; }

        [JsonProperty(PropertyName = "asignee")]
        public string Assignee { get; set; }
    }
}

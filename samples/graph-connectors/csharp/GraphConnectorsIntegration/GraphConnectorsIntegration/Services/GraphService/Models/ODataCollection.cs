namespace GraphConnectorsIntegration.Services.GraphService.Models
{
    using Newtonsoft.Json;

    public class ODataCollection<T>
    {
        [JsonProperty(PropertyName = "value")]
        public T[] Value { get; set; }
    }
}

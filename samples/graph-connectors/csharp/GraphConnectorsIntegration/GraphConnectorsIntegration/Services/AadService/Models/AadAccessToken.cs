namespace GraphConnectorsIntegration.Services.AadService.Models
{
    using Newtonsoft.Json;

    public class AadAccessToken
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }
    }
}

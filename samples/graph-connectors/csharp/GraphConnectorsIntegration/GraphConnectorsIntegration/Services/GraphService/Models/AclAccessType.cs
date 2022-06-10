namespace GraphConnectorsIntegration.Services.GraphService.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime.Serialization;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AclAccessType
    {
        [EnumMember(Value = "grant")]
        Grant,

        [EnumMember(Value = "deny")]
        Deny
    }
}

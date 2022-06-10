namespace GraphConnectorsIntegration.Services.GraphService.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime.Serialization;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AclType
    {
        [EnumMember(Value = "user")]
        User,

        [EnumMember(Value = "externalGroup")]
        ExternalGroup
    }
}

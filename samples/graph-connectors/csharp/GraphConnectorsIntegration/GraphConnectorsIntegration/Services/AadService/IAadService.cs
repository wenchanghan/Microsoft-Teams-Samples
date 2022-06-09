namespace GraphConnectorsIntegration.Services.AadService
{
    using System.Threading.Tasks;

    public interface IAadService
    {
        public Task<string> GetAccessTokenForAppAsync(string tenantId, string resourceId);
    }
}

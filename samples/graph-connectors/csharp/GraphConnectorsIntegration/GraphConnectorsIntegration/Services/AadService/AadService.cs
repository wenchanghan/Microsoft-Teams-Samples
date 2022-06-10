namespace GraphConnectorsIntegration.Services.AadService
{
    using GraphConnectorsIntegration.Services.AadService.Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class AadService : IAadService
    {
        private const string BaseUrl = "https://login.microsoftonline.com";
        private readonly HttpClient httpClient;

        public AadService(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<string> GetAccessTokenForAppAsync(string tenantId, string resourceId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new ArgumentNullException(nameof(tenantId));
            }

            if (string.IsNullOrWhiteSpace(resourceId))
            {
                throw new ArgumentNullException(nameof(resourceId));
            }

            string url = $"{BaseUrl}/{tenantId}/oauth2/v2.0/token";
            IList<KeyValuePair<string, string>> contentKeyVaulePairs = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("scope", resourceId + ".default"),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", ServiceConstants.AppId),
                new KeyValuePair<string, string>("client_secret", ServiceConstants.AppSecret),
            };

            string requestContentString = this.ConvertKeyValuePairToContentString(contentKeyVaulePairs);
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Content = new StringContent(requestContentString, Encoding.UTF8, "application/x-www-form-urlencoded");
                HttpResponseMessage response = await this.httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                AadAccessToken token = JsonConvert.DeserializeObject<AadAccessToken>(responseBody);
                return token.AccessToken;
            }
        }

        private string ConvertKeyValuePairToContentString(IList<KeyValuePair<string, string>> contentPairs)
        {
            StringBuilder contentSb = new StringBuilder();
            foreach (KeyValuePair<string, string> contentPair in contentPairs)
            {
                contentSb.Append(contentPair.Key);
                contentSb.Append('=');
                contentSb.Append(contentPair.Value);
                contentSb.Append('&');
            }

            return contentSb.ToString();
        }
    }
}

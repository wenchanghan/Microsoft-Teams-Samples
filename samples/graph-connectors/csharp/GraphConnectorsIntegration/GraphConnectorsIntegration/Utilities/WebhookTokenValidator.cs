namespace GraphConnectorsIntegration.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.IdentityModel.Protocols;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using Microsoft.IdentityModel.Tokens;

    public class WebhookTokenValidator
    {
        static readonly HashSet<string> ExpectedMicrosoftApps = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "56c1da01-2129-48f7-9355-af6d59d42766", // Graph Connector Service
            "0bf30f3b-4a52-48df-9a82-234910c4a086", // Microsoft Graph Change Tracking
        };

        private readonly IConfigurationManager<OpenIdConnectConfiguration> configurationManager;

        public WebhookTokenValidator()
        {
            this.configurationManager =
                new ConfigurationManager<OpenIdConnectConfiguration>(
                        $"https://login.microsoftonline.com/common/.well-known/openid-configuration",
                        new OpenIdConnectConfigurationRetriever());
        }

        public async Task ValidateToken(string validationToken, string tenantId, string audience, CancellationToken cancellationToken)
        {
            OpenIdConnectConfiguration configuration = await this.configurationManager.GetConfigurationAsync(cancellationToken);

            var handler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            string issuer = configuration.Issuer.Replace("{tenantid}", tenantId, StringComparison.OrdinalIgnoreCase);

            handler.ValidateToken(
                validationToken,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    RequireSignedTokens = true,
                    IssuerSigningKeys = configuration.SigningKeys,
                },
                out securityToken);

            JwtSecurityToken jwtSecurityToken = (JwtSecurityToken)securityToken;
            string appId = (string)jwtSecurityToken.Payload["appid"];

            if (!ExpectedMicrosoftApps.Contains(appId))
            {
                throw new SecurityTokenException("The token is not generated from expected Microsoft applications");
            }
        }
    }
}

<h1>Instructions to test the API without updating app manifest</h1>

1. Update `ServiceConstants.cs` with app's AAD AppId and AppSecret. Use a test AAD app with the new GraphConnectors-related API permissions configured per developer guide.
1. Locate a test tenant with tenant id. Login to the test customer tenant and grant consent to use the AAD test app.
1. Prepare the payload using the template below; update parameters in {{}}. Please also read inline comments related to "connectorsTicket" and "validationTokens".
1. Build and debug GraphConnectorsIntegration project.
1. Set a breakpoint around ValidationTokens validation so you could check the validation logics as well as force-bypassing the validation (which would fail due to mismatch between the sample validation token and the actual app-customer combo).
1. Trigger an HTTP POST call to the webhook notification processing API endpoint with the prepared notification payload. Follow debugger.
1. Once connection creation succeeds, consider calling the other API endpoint to simulate data ingestion. It has internal logics to only trigger ingestion when connection is in ready state.

<b>Template for Webhook Notification payload</b>

```
{
	"value": [
		{
			"changeType": "updated",
			"subscriptionId": "66f6f4b0-f1e3-4e5a-96ab-cd60cad5582b",
			"resource": "external",
			"clientState": null,
			"resourceData": {
				"@odata.type": "#Microsoft.Graph.connector",
				"@odata.id": "external",
				"id": "{{graphConnectorId}}",
				"state": "enabled", // or "disabled"
				"connectorsTicket": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhcHBpZCI6IjdlNDc4NDZlLTRiZWYtNGM0Mi05ODE3LWExNGU5MjI4N2YyOCIsIm5hbWUiOiJYbW4gR3JhcGggQ29ubmVjdGVkIiwiaWNvbl91cmwiOiJodHRwczovL3VzLWFwaS5hc20uc2t5cGUuY29tL3YxL29iamVjdHMvMC13dXMtZDUtYTMwMzliM2M5YzRmNDgyYzNhYmI4MzM5ZGEzZjlmMzYvdmlld3MvaW1ncHNoX2Z1bGxzaXplIiwibmJmIjoxNjUxMjU4MDczLCJleHAiOjE2ODI3OTQwNzMsImlhdCI6MTY1MTI1ODA3M30.qyQGgArmftGQLzsL1E18deiP4BwQCzuuFySQ-y8Dwig" // This is just a sample. It's meant to carry the current app's metadata like app icon, name. DO NOT USE in production env.
			},
			"subscriptionExpirationDateTime": "2022-04-30T11:47:53.3793223-07:00",
			"tenantId": "{{customerTenantId}}"
		}
	],
	"validationTokens": [
		// This is also a sample to help with testing. It's already expired and contains test data. DO NOT USE in production env.
		"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyIsImtpZCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyJ9.eyJhdWQiOiI3ZTQ3ODQ2ZS00YmVmLTRjNDItOTgxNy1hMTRlOTIyODdmMjgiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9iOTQyMTBjNy04ZWIxLTRhNDYtODE2Yi1lMjRkZmRlOTQ4ZjIvIiwiaWF0IjoxNjUxMTgyNzQ4LCJuYmYiOjE2NTExODI3NDgsImV4cCI6MTY1MTI2OTQ0OCwiYWlvIjoiRTJaZ1lPRDczdCs1bUVmWnJYTGkvOTNsa1RYdkFRPT0iLCJhcHBpZCI6IjBiZjMwZjNiLTRhNTItNDhkZi05YTgyLTIzNDkxMGM0YTA4NiIsImFwcGlkYWNyIjoiMiIsImlkcCI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0L2I5NDIxMGM3LThlYjEtNGE0Ni04MTZiLWUyNGRmZGU5NDhmMi8iLCJvaWQiOiI2OGFiYjk4NC05MzEzLTQ2YzEtODljNS1lNjE4NzAzN2UxYjMiLCJyaCI6IjAuQVh3QXh4QkN1YkdPUmtxQmEtSk5fZWxJOG02RVIzN3ZTMEpNbUJlaFRwSW9meWg4QUFBLiIsInN1YiI6IjY4YWJiOTg0LTkzMTMtNDZjMS04OWM1LWU2MTg3MDM3ZTFiMyIsInRpZCI6ImI5NDIxMGM3LThlYjEtNGE0Ni04MTZiLWUyNGRmZGU5NDhmMiIsInV0aSI6IklQci1LWU5ibDBtVm5oOElKVmdFQUEiLCJ2ZXIiOiIxLjAifQ.piTMj85lndTx2MlUb86CfwpSu5SZO8pCT2tpxr7dDfqwcC7hzOJdrXRGJhlzLcCioTuMqLj6EerupRUuEZDAoAzQRDpifJEx6PypR4UtJuiFgG-5JDznC3reOkYAmbbJz0wIPrszrIa4l_uvfJFZZK_BYKDoo8u6qFCwc-JOAUZOFDmRBeH2_ej_EX5vDEWJw9CkzeHVRaH3iX8h5NrotJyIN0_w0phBp5SLfsipArPv_090r-IknknyBPcbd3S_U8LfX_-om3sEvitYRernKxXP0w2F2lMQCpMxndTuVmPqGBCatecVmc_O8Sy75v_pymjIaOlAMnTrFG5laETv5A"
	]
}
```
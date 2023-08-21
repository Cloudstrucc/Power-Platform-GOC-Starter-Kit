
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;

string resource = "https://edmps-dev.api.crm3.dynamics.com";
var clientId = "dbe024e7-ea5c-4712-8a8e-4efa6da46fce";                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        45987d";
var redirectUri = "https://login.onmicrosoft.com"; // Loopback for the interactive login.

// MSAL authentication
var authBuilder = PublicClientApplicationBuilder.Create(clientId)
    .WithAuthority(AadAuthorityAudience.AzureAdMultipleOrgs)
    .WithRedirectUri(redirectUri)
    .Build();
var scope = resource + "/.default";
string[] scopes = { scope };

Microsoft.Identity.Client.AuthenticationResult token =
    authBuilder.AcquireTokenInteractive(scopes).ExecuteAsync().Result;

// Set up the HTTP client
var client = new HttpClient
{
    BaseAddress = new Uri(resource + "/api/data/v9.2/"),
    Timeout = new TimeSpan(0, 2, 0)  // Standard two minute timeout.
};

HttpRequestHeaders headers = client.DefaultRequestHeaders;
headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
headers.Add("OData-MaxVersion", "4.0");
headers.Add("OData-Version", "4.0");
headers.Accept.Add(
    new MediaTypeWithQualityHeaderValue("application/json"));

// Web API call
var response = client.GetAsync("WhoAmI").Result;
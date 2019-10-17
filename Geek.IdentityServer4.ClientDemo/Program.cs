using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace Geek.IdentityServer4.ClientDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();
            var tokenUrl = (await client.GetDiscoveryDocumentAsync("http://localhost:5000")).TokenEndpoint;
            var token = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                RequestUri = new Uri(tokenUrl),
                ClientId = "cli",
                ClientSecret = "",
                Scope = "api1"
            });
            Console.WriteLine(token.Json);
            Console.WriteLine(token.AccessToken);
            client.SetBearerToken(token.AccessToken);
            var response = await client.GetStringAsync("http://localhost:5001/identity");
            Console.WriteLine(response);
        }
    }
}

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
            await ClientTests();
            // await RoClientTests();
        }

        private static async Task RoClientTests()
        {
            var disco = await DiscoveryClient.GetAsync("https://sso.neverc.cn"); // Policy.RequireHttps = true
            var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("alice", "password", "api1");

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            var response = await client.GetAsync("http://localhost:5001/identity");
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(JArray.Parse(content));
        }

        private static async Task ClientTests()
        {
            var disco = await DiscoveryClient.GetAsync("https://sso.neverc.cn"); // Policy.RequireHttps = true
            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            var response = await client.GetAsync("http://localhost:5001/identity");
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(JArray.Parse(content));
        }
    }
}

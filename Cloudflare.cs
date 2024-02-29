//Ignore Spelling:proxied
using CloudflareUpdateDNS.Models;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace CloudflareUpdateDNS;
public static class Cloudflare
{
    static readonly HttpClient client = new HttpClient();
    public static bool InitialCheck = false;
    public static string IPInCloudflare = "";

    //These calls are also saved in Postman under lilo.co.il section

    public static async Task<DNSRecordsResponse?> GetDomains(string zoneId, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.cloudflare.com/client/v4/zones/{zoneId}/dns_records");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        HttpResponseMessage response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            DNSRecordsResponse? responseBody = await response.Content.ReadFromJsonAsync<DNSRecordsResponse>();
            Console.WriteLine(JsonSerializer.Serialize(responseBody));
            return responseBody;
        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}");
            return null;
        }
    }

    public static async Task<GetDomainDetailsResponse?> GetDomainDetails(string zoneId, string token, string domain)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.cloudflare.com/client/v4/zones/{zoneId}/dns_records/{domain}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        HttpResponseMessage response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            GetDomainDetailsResponse? responseBody = await response.Content.ReadFromJsonAsync<GetDomainDetailsResponse>();
            Console.WriteLine(JsonSerializer.Serialize(responseBody));
            return responseBody;
        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}");
            return null;
        }
    }

    public static async Task UpdateDomainIP(string zoneId, string token, string domainId, string domain, string newIP)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"https://api.cloudflare.com/client/v4/zones/{zoneId}/dns_records/{domainId}");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //var content = new StringContent("{\"content\": \"81.218.126.242\",\"name\": \"raspberry.lilo.co.il\",\"proxied\": true,\"type\": \"A\"}", Encoding.UTF8, "application/json");
        var content = new StringContent($"{{\"content\": \"{newIP}\",\"name\": \"{domain}\",\"proxied\": true,\"type\": \"A\"}}", Encoding.UTF8, "application/json");
        request.Content = content;

        HttpResponseMessage response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            Cloudflare.IPInCloudflare = newIP;
            GetDomainDetailsResponse? responseBody = await response.Content.ReadFromJsonAsync<GetDomainDetailsResponse>();
            Console.WriteLine(JsonSerializer.Serialize(responseBody));
        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}");
        }
    }



}





class Program
{


}

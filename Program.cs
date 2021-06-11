using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;

//https://www.weather.gov/documentation/services-web-api#
HttpClient client = new HttpClient();
var lattitude = args.Length >= 1 ? args[0] : "39.1";
var longitude = args.Length >= 2 ? args[1] : "-84.5";

string username = "demo-backend-client", password = "MJlO3binatD9jk1";
using (var request = new HttpRequestMessage(HttpMethod.Post, "https://login-demo.curity.io/oauth/v2/oauth-token"))
{
    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));
    request.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "grant_type", "client_credentials" }, { "scope", "read write" } });
    using (var response = await client.SendAsync(request))
    {
        Console.WriteLine($"oauth client request was: {response.StatusCode}");
        var credJO = JObject.Parse(response.Content.ReadAsStringAsync().Result);
        Console.WriteLine($"\tbearer token: {credJO["access_token"]}");
    }
}
string gridX, gridY;
using (var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.weather.gov/points/{lattitude},{longitude}"))
{
    request.Headers.Add("User-Agent", "(563266e5-d17a-4aa1-981b-38b7e05c8697, gregolm@gmail.com)");
    using (var response = await client.SendAsync(request))
    {
        var gridInfo = JObject.Parse(await response.Content.ReadAsStringAsync());
        gridX = gridInfo["properties"]["gridX"].ToString();
        gridY = gridInfo["properties"]["gridY"].ToString();
        Console.WriteLine($"Grid info was: ({gridX},{gridY})");
    }
}
JObject wxInfo;
using (var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.weather.gov/gridpoints/TOP/{gridX},{gridY}/forecast"))
{
    request.Headers.Add("User-Agent", "(563266e5-d17a-4aa1-981b-38b7e05c8697, gregolm@gmail.com)");
    using (var response = await client.SendAsync(request))
    {
        Console.WriteLine($"Status was: {response.StatusCode}");
        wxInfo = JObject.Parse(await response.Content.ReadAsStringAsync());
    }
}
var periods = 
    from period in wxInfo["properties"]["periods"]
    select new
    {
        name = period["name"],
        title = period["shortForecast"],
        forecast = period["detailedForecast"]
    };

Console.WriteLine("Forecast:");
foreach(var period in periods)
{
    Console.WriteLine($"{period.name} => {period.title}\n\t{period.forecast}");
}

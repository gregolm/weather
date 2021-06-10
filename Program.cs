using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;

//https://www.weather.gov/documentation/services-web-api#
HttpClient client = new HttpClient();
var lattitude = args.Length >= 1 ? args[0] : "39.1";
var longitude = args.Length >= 2 ? args[1] : "-84.5";
var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.weather.gov/points/{lattitude},{longitude}");
request.Headers.Add("User-Agent", "(563266e5-d17a-4aa1-981b-38b7e05c8697, gregolm@gmail.com)");
var response = await client.SendAsync(request);
var gridInfo = JObject.Parse(await response.Content.ReadAsStringAsync());
var gridX = gridInfo["properties"]["gridX"];
var gridY = gridInfo["properties"]["gridY"];
Console.WriteLine($"Grid info was: ({gridX},{gridY})");
request = new HttpRequestMessage(HttpMethod.Get, $"https://api.weather.gov/gridpoints/TOP/{gridX},{gridY}/forecast");
request.Headers.Add("User-Agent", "(563266e5-d17a-4aa1-981b-38b7e05c8697, gregolm@gmail.com)");
response = await client.SendAsync(request);
Console.WriteLine($"Status was: {response.StatusCode}");
var wxInfo = JObject.Parse(await response.Content.ReadAsStringAsync());
var periods = from period in wxInfo["properties"]["periods"]
                select new { name = period["name"], 
                            title = period["shortForecast"], 
                            forecast = period["detailedForecast"] };
Console.WriteLine("Forecast:");
foreach(var period in periods)
{
    Console.WriteLine($"{period.name} => {period.title}\n\t{period.forecast}");
}


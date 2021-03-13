using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace deezer_client
{
    public partial class Deezer
    {
        private readonly string token;
        public HttpClient client = new HttpClient();
        public string cookies;
        public Uri uri = new Uri("https://www.deezer.com");

        private Deezer(string cookies, string token)
        {
            this.cookies = cookies;
            this.token = token;
            client.DefaultRequestHeaders.Add("Cookie", cookies);
        }

        public int downloadedSongs { get; set; }

        public async Task<string> ApiCallMethod<T>(string method, T content)
        {
            // TODO: Add error handling
            const string apiurl = "https://www.deezer.com/ajax/gw-light.php";

            var builder = new UriBuilder(apiurl);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["api_version"] = "1.0";
            query["api_token"] = token;
            query["input"] = "3";
            query["method"] = method;
            builder.Query = query.ToString();
            var url = builder.ToString();
            var contentJson = new StringContent(JsonConvert.SerializeObject(content));
            var res = await client.PostAsync(url, contentJson);
            var resString = await res.Content.ReadAsStringAsync();
            var resultJson = JObject.Parse(resString).SelectToken("$.results")?.ToString();

            return resultJson;
        }
        public async Task<string> ApiCallMethodRaw(string method, string content)
        {
            // TODO: Add error handling
            const string apiurl = "https://www.deezer.com/ajax/gw-light.php";

            var builder = new UriBuilder(apiurl);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["api_version"] = "1.0";
            query["api_token"] = token;
            query["input"] = "3";
            query["method"] = method;
            builder.Query = query.ToString();
            var url = builder.ToString();
            var res = await client.PostAsync(url, new StringContent(content));
            var resString = await res.Content.ReadAsStringAsync();
            var resultJson = JObject.Parse(resString).SelectToken("$.results")?.ToString();

            return resultJson;
        }


        public async Task<string> LegacyApiCallMethod(string method, Dictionary<string, string> parameters,
            bool isArray)
        {
            // TODO: Add error handling
            const string apiurl = "https://api.deezer.com/";

            var builder = new UriBuilder(apiurl + method);
            var query = HttpUtility.ParseQueryString(builder.Query);
            foreach (var parameter in parameters) query[parameter.Key] = parameter.Value;
            builder.Query = query.ToString();
            var url = builder.ToString();

            var res = await client.GetAsync(url);
            var resString = await res.Content.ReadAsStringAsync();
            if (isArray) resString = JObject.Parse(resString).SelectToken("$.data")?.ToString();

            return resString;
        }

        public static async Task<Deezer> Login(string arl)
        {
            string GetCookie(HttpResponseMessage message)
            {
                message.Headers.TryGetValues("Set-Cookie", out var setCookie);
                var setCookieString = setCookie.Reverse().Aggregate("", (s, s1) =>
                {
                    var keyAndCookie = s1.Split(";").First().Split("=");
                    if (keyAndCookie[0] == "account_id")
                        return s;
                    var currentString = $"{keyAndCookie[0]}={keyAndCookie[1]}; ";
                    return s + currentString;
                });
                return setCookieString;
            }

            var uri = new Uri(
                "https://www.deezer.com/ajax/gw-light.php?api_version=1.0&api_token=null&input=3&method=deezer.getUserData");
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Cookie", $"arl={arl}");
            var data = await client.GetAsync(uri);
            var contentString = await data.Content.ReadAsStringAsync();
            var contentJSON = JObject.Parse(contentString);
            var token = contentJSON.SelectToken("$.results.checkForm").ToObject<string>();

            var cookie = GetCookie(data);

            return new Deezer(cookie, token);
        }
    }
}
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace spotify_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Spotify : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public Spotify()
        {
            _httpClient = new HttpClient();
        }

        //get token, expires in 1 hour - all reqs use this token
        private async Task<string> getToken()
        {
            var token_uri = "https://accounts.spotify.com/api/token";
            
            //send client id and secret
            var formData = new Dictionary<string, string>();
            formData.Add("grant_type", "client_credentials");
            formData.Add("client_id", "e6ffef3a3762474a9861ebcfe3bad0ed");
            formData.Add("client_secret", "04d160a95fbf4cb495a0c67b22149bfb");
            var urlEnconded = new FormUrlEncodedContent(formData);

            var response = await _httpClient.PostAsync(token_uri, urlEnconded);

            //System.Diagnostics.Debug.WriteLine(response.Content.ReadAsStringAsync().Result);

            return response.Content.ReadAsStringAsync().Result.ToString();

        }

        [HttpGet]
        public async Task<string> search(string q)
        {
            var jsonString = await getToken();
            //System.Diagnostics.Debug.WriteLine(jsonString);
            var auth = JsonConvert.DeserializeObject<Auth>(jsonString);
            //var token = auth.access_token;

            var search = Uri.EscapeDataString(q);
            var types = "track"; //possibles types:album,artist,playlist,track
            var url = $"https://api.spotify.com/v1/search?q={search}&type={Uri.EscapeDataString(types)}";
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", auth.access_token);
            //System.Diagnostics.Debug.WriteLine(url);

            var response = await _httpClient.GetAsync(url);

            var responseJson = JObject.Parse(
                response.Content.ReadAsStringAsync().Result.ToString());

            System.Diagnostics.Debug.WriteLine(responseJson);

            //using Linq and Newtonsoft.Json.Linq to query a
            var tracks = responseJson["tracks"]["items"].
                Select(i => new 
                Track(
                    (string)i["id"], 
                    (string)i["name"], 
                    (int)i["popularity"]
                    )).
                OrderByDescending(x => x.Popularity).
                ToList();

            foreach (var item in tracks)
            {

                //System.Diagnostics.Debug.WriteLine($"{item["name"]}");
                System.Diagnostics.Debug.WriteLine($"{item},");
            }

            return JsonConvert.SerializeObject(tracks);

        }
    }
}

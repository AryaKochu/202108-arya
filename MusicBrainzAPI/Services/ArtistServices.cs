using MusicBrainzAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MusicBrainzAPI.Services
{
    public class ArtistServices : IArtistService
    {
        private readonly IHttpClientFactory _clientFactory;

        public ArtistServices(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        
        public async Task<Response> GetArtistsAsync(string name)
        {
            var response = new Response();
            var queryString = name.Replace(" ", "%20");
            var artists = new List<Artist>();
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"artist/?query=artist:{queryString}&fmt=json");

            AddHeaders(request);

            var client = _clientFactory.CreateClient("musicBrainzApi");

            var result = await client.SendAsync(request);

            if (result.IsSuccessStatusCode)
            {
                var responseString = result.Content.ReadAsStringAsync().Result;
                var jObj = JObject.Parse(responseString);

                foreach (var artist in jObj.SelectToken("artists"))
                {
                    artists.Add(new Artist
                    {
                        Country = artist.SelectToken("country")?.ToString(),
                        Name = artist.SelectToken("name")?.ToString(),
                        Gender = artist.SelectToken("gender")?.ToString()
                    });
                }
                var exactMatchArtists = artists.Where(a => a.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)).ToList();

                if (exactMatchArtists.Count > 1)
                    response.Artists = exactMatchArtists;

                else if (exactMatchArtists.Count == 1)
                {
                    var releases = await GetReleasesOfGivenArtist(client, queryString);
                    response.Artists = exactMatchArtists;
                    response.Releases = releases;
                }
            }
            else
            {
                return new Response { Error = "Something went wrong" };
            }
            return response;

        }

        private async Task<IList<Release>>  GetReleasesOfGivenArtist(HttpClient client, string queryString)
        {
            var releases = new List<Release>();
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"release/?query=release:{queryString}&fmt=json");

            AddHeaders(request);

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseString = response.Content.ReadAsStringAsync().Result;
                var jObj = JObject.Parse(responseString);

                foreach (var artist in jObj.SelectToken("releases"))
                {
                    releases.Add(new Release
                    {
                        Country = artist.SelectToken("country")?.ToString(),
                        Date = artist.SelectToken("date")?.ToString(),
                        Title = artist.SelectToken("title")?.ToString()
                    });
                }
            }
                return releases;

        }

        private void AddHeaders(HttpRequestMessage request)
        {
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("User-Agent", "GetArtist-TestApi");
        }
    }
}

using MusicBrainzAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MusicBrainzAPI.Services.Interfaces;

namespace MusicBrainzAPI.Services
{
    public class ArtistServices : IArtistService
    {
        private readonly IHttpClientService _clientService;

        public ArtistServices(IHttpClientService clientService)
        {
            _clientService = clientService;
        }

        
        public async Task<Response> GetArtistsAsync(string name)
        {
            var response = new Response();
            var queryString = name.Replace(" ", "%20");
            var artists = new List<Artist>();
            var result = await _clientService.GetArtistDetails(queryString);
            if (result.IsSuccessStatusCode)
            {
                var responseString = result.Content.ReadAsStringAsync().Result;
                var jObj = JObject.Parse(responseString);
                var artistDetails = jObj.SelectToken("artists");
                if (artistDetails != null)
                {
                    foreach (var artist in artistDetails)
                    {
                        artists.Add(new Artist
                        {
                            Country = artist?.SelectToken("country")?.ToString(),
                            Name = artist?.SelectToken("name")?.ToString(),
                            Gender = artist?.SelectToken("gender")?.ToString()
                        });
                    }
                }
                var exactMatchArtists = artists.Where(a => a.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)).ToList();

                if (exactMatchArtists.Count > 1)
                    response.Artists = exactMatchArtists;

                else if (exactMatchArtists.Count == 1)
                {
                    var releases = await GetReleasesOfGivenArtist(queryString);
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

        private async Task<IList<Release>>  GetReleasesOfGivenArtist(string queryString)
        {
            var releases = new List<Release>();
            /*var request = new HttpRequestMessage(HttpMethod.Get,
                $"release/?query=release:{queryString}&fmt=json");

            var response = await client.SendAsync(request);*/
            var result = await _clientService.GetReleasesByArtist(queryString);

            if (result.IsSuccessStatusCode)
            {
                var responseString = result.Content.ReadAsStringAsync().Result;
                var jObj = JObject.Parse(responseString);

                foreach (var artist in jObj.SelectToken("releases"))
                {
                    releases.Add(new Release
                    {
                        Country = artist?.SelectToken("country")?.ToString(),
                        Date = artist?.SelectToken("date")?.ToString(),
                        Title = artist?.SelectToken("title")?.ToString()
                    });
                }
            }
            return releases;

        }
    }
}

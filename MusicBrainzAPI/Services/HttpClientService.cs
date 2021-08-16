using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MusicBrainzAPI.Services.Interfaces;

namespace MusicBrainzAPI.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _client;

        public HttpClientService(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("musicBrainzApi");
        }

        public async Task<HttpResponseMessage> GetArtistDetails(string queryString)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"artist/?query=artist:{queryString}&fmt=json");

           return await _client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> GetReleasesByArtist(string queryString)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"release/?query=release:{queryString}&fmt=json");

            return await _client.SendAsync(request);
        }
    }
}

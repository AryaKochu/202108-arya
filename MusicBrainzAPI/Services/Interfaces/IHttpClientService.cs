using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MusicBrainzAPI.Services.Interfaces
{
    public interface IHttpClientService
    {
        Task<HttpResponseMessage> GetArtistDetails(string queryString);
        Task<HttpResponseMessage> GetReleasesByArtist(string queryString);
    }
}

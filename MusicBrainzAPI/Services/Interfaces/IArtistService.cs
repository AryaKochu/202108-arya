using MusicBrainzAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicBrainzAPI.Services
{
    public interface IArtistService
    {
        Task<Response> GetArtistsAsync(string name);

    }
}

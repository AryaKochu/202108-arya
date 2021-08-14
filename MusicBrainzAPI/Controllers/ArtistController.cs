using Microsoft.AspNetCore.Mvc;
using MusicBrainzAPI.Services;
using System.Threading.Tasks;

namespace MusicBrainzAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArtistController : ControllerBase
    {
        private IArtistService _artistService;

        public ArtistController(IArtistService artistService)
        {
            _artistService = artistService;
        }

        [HttpGet("{artist}")]
        public async Task<ObjectResult> GetArtist(string artist)
        {
            if (string.IsNullOrWhiteSpace(artist)) return new BadRequestObjectResult("The name entered is invalid.");
            var result = await _artistService.GetArtistsAsync(artist);
            return new OkObjectResult(result);
        }
    }
}

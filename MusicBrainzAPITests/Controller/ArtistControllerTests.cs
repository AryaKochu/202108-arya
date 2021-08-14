using MusicBrainzAPI.Controllers;
using MusicBrainzAPI.Services;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MusicBrainzAPITests.Controller
{
    public class ArtistControllerTests
    {
        private IArtistService _artistService;

        public ArtistControllerTests()
        {
            _artistService = Substitute.For<IArtistService>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task GivenAnInvalidName_ReturnBadRequest(string artist)
        {
            //Arrange
            var controller = new ArtistController(_artistService);

            //Action
            var result = await controller.GetArtist(artist);

            //Assert
           Assert.Equal(result.StatusCode.Value, Convert.ToInt32(HttpStatusCode.BadRequest));
        }
    }
}

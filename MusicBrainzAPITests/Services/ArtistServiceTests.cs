using MusicBrainzAPI.Services;
using MusicBrainzAPITests.TestHelpers;
using NSubstitute;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MusicBrainzAPI.Services.Interfaces;
using Xunit;

namespace MusicBrainzAPITests
{
    public class ArtistServiceTests
    {
        private readonly string _name = "Taylor Swift";
        private IHttpClientFactory _clientFactory;
        private HttpClientService _clientService;
        private ArtistServices _artistService;



        public ArtistServiceTests()
        {
            _clientFactory = Substitute.For<IHttpClientFactory>();

            //var fakeClient = new HttpClient();
            //fakeClient.BaseAddress = new Uri("https://localhost");
            //_clientFactory.CreateClient("musicBrainzApi").Returns(fakeClient);

            _clientService = Substitute.For<HttpClientService>(_clientFactory);
            //_artistService = new ArtistServices(_clientService);
        }

        [Fact]
        public async Task GivenValidArtistName_ReturnAllArtistsMatched()
        {
            //Arrange

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(await File.ReadAllTextAsync("Mock/ArtistResponse.json"),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json)

            };
            var messageHandler = new FakeHttpMessageHandler(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(await File.ReadAllTextAsync("Mock/ArtistResponse.json"),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json)
            });
             var client = new HttpClient(messageHandler);
             _clientFactory.CreateClient("musicBrainzApi").Returns(client);

            _clientService = new HttpClientService(_clientFactory);
            _clientService.GetArtistDetails("Taylor%20Swift").Returns(response);

            _artistService = new ArtistServices(_clientService);
            //Action

            var result = await _artistService.GetArtistsAsync(_name);

            //Assert
            Assert.Equal(2, result.Artists.Count);
        }

        [Fact]
        public async Task GivenValidArtistName_BackendReturnedNot200_ReturnError()
        {
            //Arrange

            var messageHandler = new FakeHttpMessageHandler(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadGateway,
                Content = new StringContent(File.ReadAllText("Mock/ArtistResponse.json"),
                   Encoding.UTF8,
                   MediaTypeNames.Application.Json)
            });

            var fakeClient = new HttpClient(messageHandler);
            fakeClient.BaseAddress = new Uri("https://localhost");
            _clientFactory.CreateClient("musicBrainzApi").Returns(fakeClient);

            //Action

            var result = await _artistService.GetArtistsAsync(_name);

            //Assert
            Assert.Equal("Something went wrong", result.Error);
        }
    }
}

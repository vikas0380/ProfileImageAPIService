using Moq;
using Moq.Protected;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ProfileImageBusinessLayer.Tests;

[TestFixture]
public class AvatarServiceTests
{
     private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;

    [SetUp]
        public void Setup()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            var expectedImageUrl = "https://example.com/image.jpg";

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"url\": \"" + expectedImageUrl + "\"}"),
                });
        }
    [Test]
    public async Task GetImageUrl_LastDigit6To9_ReturnsImageUrlFromExternalApi()
    {
        var expectedImageUrl = "https://my-json-server.typicode.com/ck-pacificdev/tech-test/images/6";
        // Arrange
         _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"url\": \"" + expectedImageUrl + "\"}"),
                });
        var imageRepositoryMock = new Mock<IImageRepository>();
        var avatarService = new AvatarService(imageRepositoryMock.Object,_httpClient);

        // Act
        var imageUrl = await avatarService.GetImageUrl("123456");

        // Assert
        Assert.AreEqual(expectedImageUrl, imageUrl);
    }

    [Test]
    public async Task GetImageUrl_LastDigit1To5_ReturnsImageUrlFromRepository()
    {
        // Arrange
        var imageRepositoryMock = new Mock<IImageRepository>();
        imageRepositoryMock.Setup(x => x.GetImageUrl('3')).ReturnsAsync("https://example.com/image3.jpg");
        var avatarService = new AvatarService(imageRepositoryMock.Object,_httpClient);

        // Act
        var imageUrl = await avatarService.GetImageUrl("123");

        // Assert
        Assert.AreEqual("https://example.com/image3.jpg", imageUrl);
    }

    [Test]
    public async Task GetImageUrl_ContainsVowel_ReturnsImageUrlFromExternalApi()
    {
        // Arrange
        var imageRepositoryMock = new Mock<IImageRepository>();
         imageRepositoryMock.Setup(x => x.GetImageUrl('3')).ReturnsAsync("https://api.dicebear.com/8.x/pixel-art/png?seed=vowel&size=150");
        var avatarService = new AvatarService(imageRepositoryMock.Object,_httpClient);

        // Act
        var imageUrl = await avatarService.GetImageUrl("aeiou123");

        // Assert
        Assert.IsTrue(imageUrl.StartsWith("https://api.dicebear.com/8.x/pixel-art/png?seed=vowel&size=150"));
    }

    [Test]
    public async Task GetImageUrl_ContainsNonAlphanumericCharacter_ReturnsImageUrlWithRandomSeed()
    {
        // Arrange
         var expectedImageUrl = "https://api.dicebear.com/8.x/pixel-art/png?seed=";
        // Arrange
         _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"url\": \"" + expectedImageUrl + "\"}"),
                });
        var imageRepositoryMock = new Mock<IImageRepository>();
        var avatarService = new AvatarService(imageRepositoryMock.Object,_httpClient);

        // Act
        var imageUrl = await avatarService.GetImageUrl("user!234!");

        // Assert
        Assert.IsTrue(imageUrl.StartsWith("https://api.dicebear.com/8.x/pixel-art/png?seed="));
    }

    [Test]
    public async Task GetImageUrl_NoMatchingScenario_ReturnsDefaultImageUrl()
    {
        var expectedImageUrl = "https://api.dicebear.com/8.x/pixel-art/png?seed=default&size=150";
        // Arrange
         _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"url\": \"" + expectedImageUrl + "\"}"),
                });
        // Arrange
        var imageRepositoryMock = new Mock<IImageRepository>();
        var avatarService = new AvatarService(imageRepositoryMock.Object,_httpClient);

        // Act
        var imageUrl = await avatarService.GetImageUrl("1234567890");

        // Assert
        Assert.IsTrue(imageUrl.StartsWith("https://api.dicebear.com/8.x/pixel-art/png?seed=default&size=150"));
    }
}

using NUnit.Framework;
using System.Threading.Tasks;
using Moq;

namespace ProfileImageWeb.Tests;

[TestFixture]
public class AvatarControllerTests
{
    [Test]
    public async Task GetAvatar_LastDigit6To9_ReturnsCorrectImageUrl()
    {
        // Arrange
        var avatarServiceMock = new Mock<IAvatarService>();
        avatarServiceMock.Setup(x => x.GetImageUrl("123456")).ReturnsAsync("https://my-json-server.typicode.com/ck-pacificdev/tech-test/images/6");
        var controller = new AvatarController(avatarServiceMock.Object);

        // Act
        var result = await controller.GetAvatar("123456");

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result is Microsoft.AspNetCore.Mvc.OkObjectResult);
        var okResult = result as Microsoft.AspNetCore.Mvc.OkObjectResult;
        Assert.AreEqual("https://my-json-server.typicode.com/ck-pacificdev/tech-test/images/6", ((ProfileResponse)okResult.Value).Url);
    }

    [Test]
    public async Task GetAvatar_LastDigit1To5_ReturnsCorrectImageUrl()
    {
        // Arrange
        var avatarServiceMock = new Mock<IAvatarService>();
        avatarServiceMock.Setup(x => x.GetImageUrl("123")).ReturnsAsync("https://example.com/image3.jpg");
        var controller = new AvatarController(avatarServiceMock.Object);

        // Act
        var result = await controller.GetAvatar("123");

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result is Microsoft.AspNetCore.Mvc.OkObjectResult);
        var okResult = result as Microsoft.AspNetCore.Mvc.OkObjectResult;
        Assert.AreEqual("https://example.com/image3.jpg", ((ProfileResponse)okResult.Value).Url);
    }

    [Test]
    public async Task GetAvatar_ContainsVowel_ReturnsCorrectImageUrl()
    {
        // Arrange
        var avatarServiceMock = new Mock<IAvatarService>();
        avatarServiceMock.Setup(x => x.GetImageUrl("aeiou123")).ReturnsAsync("https://api.dicebear.com/8.x/pixel-art/png?seed=vowel&size=150");
        var controller = new AvatarController(avatarServiceMock.Object);

        // Act
        var result = await controller.GetAvatar("aeiou123");

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result is Microsoft.AspNetCore.Mvc.OkObjectResult);
        var okResult = result as Microsoft.AspNetCore.Mvc.OkObjectResult;
        Assert.IsTrue(((ProfileResponse)okResult.Value).Url.StartsWith("https://api.dicebear.com/8.x/pixel-art/png?seed=vowel&size=150"));
    }

    [Test]
    public async Task GetAvatar_ContainsNonAlphanumericCharacter_ReturnsCorrectImageUrl()
    {
        // Arrange
        var avatarServiceMock = new Mock<IAvatarService>();
        avatarServiceMock.Setup(x => x.GetImageUrl("user!234")).ReturnsAsync("https://api.dicebear.com/8.x/pixel-art/png?seed=random&size=150");
        var controller = new AvatarController(avatarServiceMock.Object);

        // Act
        var result = await controller.GetAvatar("user!234");

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result is Microsoft.AspNetCore.Mvc.OkObjectResult);
        var okResult = result as Microsoft.AspNetCore.Mvc.OkObjectResult;
        Assert.IsTrue(((ProfileResponse)okResult.Value).Url.StartsWith("https://api.dicebear.com/8.x/pixel-art/png?seed=random&size=150"));
    }

    [Test]
    public async Task GetAvatar_NoMatchingScenario_ReturnsDefaultImageUrl()
    {
        // Arrange
        var avatarServiceMock = new Mock<IAvatarService>();
        avatarServiceMock.Setup(x => x.GetImageUrl("1234567890")).ReturnsAsync("https://api.dicebear.com/8.x/pixel-art/png?seed=default&size=150");
        var controller = new AvatarController(avatarServiceMock.Object);

        // Act
        var result = await controller.GetAvatar("1234567890");

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result is Microsoft.AspNetCore.Mvc.OkObjectResult);
        var okResult = result as Microsoft.AspNetCore.Mvc.OkObjectResult;
        Assert.IsTrue(((ProfileResponse)okResult.Value).Url.StartsWith("https://api.dicebear.com/8.x/pixel-art/png?seed=default&size=150"));
    }
}
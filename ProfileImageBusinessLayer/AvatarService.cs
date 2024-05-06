using System.Text.Json;

namespace ProfileImageBusinessLayer;
public class AvatarService : IAvatarService
{
    private readonly IImageRepository _imageRepository;
    private readonly HttpClient _httpClient;

    public AvatarService(IImageRepository imageRepository,HttpClient httpClient)
    {
        _imageRepository = imageRepository;
         _httpClient = httpClient;
    }

    public async Task<string> GetImageUrl(string userIdentifier)
    {
        char lastDigit = userIdentifier.Last();

        if (new[] { '6', '7', '8', '9' }.Contains(lastDigit))
        {
            return await GetImageUrlFromExternalApi($"https://my-json-server.typicode.com/ck-pacificdev/tech-test/images/{lastDigit}");
        }
        else if (new[] { '1', '2', '3', '4', '5' }.Contains(lastDigit))
        {
            return await _imageRepository.GetImageUrl(lastDigit);
        }
        else if (userIdentifier.Any(char.IsLetter) && userIdentifier.Any(char.IsDigit))
        {
            return await GetImageUrlFromExternalApi("https://api.dicebear.com/8.x/pixel-art/png?seed=vowel&size=150");
        }
        else if (userIdentifier.Any(c => !char.IsLetterOrDigit(c)))
        {
            Random random = new Random();
            int randomNumber = random.Next(1, 6);
            return await GetImageUrlFromExternalApi($"https://api.dicebear.com/8.x/pixel-art/png?seed={randomNumber}&size=150");
        }
        else
        {
            return await GetImageUrlFromExternalApi("https://api.dicebear.com/8.x/pixel-art/png?seed=default&size=150");
        }
    }

    private async Task<string> GetImageUrlFromExternalApi(string apiUrl)
    {
        var response = await _httpClient.GetAsync(apiUrl);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            return json.RootElement.GetProperty("url").GetString();
        }
        else
        {
            // Handle error
            return null;
        }
    }
}

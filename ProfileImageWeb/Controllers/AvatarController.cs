using Microsoft.AspNetCore.Mvc;

public class AvatarController : ControllerBase
{
    private readonly IAvatarService _avatarService;

    public AvatarController(IAvatarService avatarService)
    {
        _avatarService = avatarService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAvatar(string userIdentifier)
    {
        string imageUrl = await _avatarService.GetImageUrl(userIdentifier);
        return Ok(new ProfileResponse{ Url = imageUrl });
    }
}

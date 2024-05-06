public interface IAvatarService
{
    Task<string> GetImageUrl(string userIdentifier);
}

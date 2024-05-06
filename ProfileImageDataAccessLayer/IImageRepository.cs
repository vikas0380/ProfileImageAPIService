public interface IImageRepository
{
    Task<string> GetImageUrl(char lastDigit);
}

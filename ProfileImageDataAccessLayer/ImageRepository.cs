
using System.Data.SQLite;
using System.Threading.Tasks;

namespace ProfileImageDataAccessLayer;
public class ImageRepository : IImageRepository
{
    private readonly string _connectionString;

    public ImageRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<string> GetImageUrl(char lastDigit)
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT imageUrl FROM images WHERE id = @lastDigit";
                command.Parameters.AddWithValue("@lastDigit", lastDigit.ToString());
                var result = await command.ExecuteScalarAsync();
                return result?.ToString();
            }
        }
    }
}


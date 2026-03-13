using GameStore.Api.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GameStore.Api.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly string _connectionString;

        public GameRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Missing DefaultConnection");
        }


        public async Task<bool> TestConnectionAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection.State == System.Data.ConnectionState.Open;
        }


        public async Task ClearAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("DELETE FROM Games", connection);

            await command.ExecuteNonQueryAsync();
        }


        public async Task<List<Game>> GetAllAsync()
        {
            var games = new List<Game>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT Id, Title, Thumbnail, ShortDescription, GameUrl, Genre, Platform, Publisher, Developer, ReleaseDate, FreeToGameProfileUrl FROM Games",
                connection);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var game = new Game
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Thumbnail = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    ShortDescription = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    GameUrl = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    Genre = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    Platform = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    Publisher = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    Developer = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                    ReleaseDate = reader.IsDBNull(9)
                        ? null : DateOnly.FromDateTime(reader.GetDateTime(9)),
                    FreeToGameProfileUrl = reader.IsDBNull(10)
                        ? string.Empty : reader.GetString(10)
                };
                games.Add(game);
            }

            return games;
        }


        public async Task<Game?> GetByIdAsync(int Id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT TOP (1) Id, Title, Thumbnail, ShortDescription, GameUrl, Genre, Platform, Publisher, Developer, ReleaseDate, FreeToGameProfileUrl FROM Games " +
                "WHERE Id = @Id",
                connection);

            command.Parameters.Add("@Id", SqlDbType.Int).Value = Id;

            using var reader = await command.ExecuteReaderAsync();

            if(await reader.ReadAsync())
            {
                var game = new Game
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Thumbnail = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    ShortDescription = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    GameUrl = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    Genre = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    Platform = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    Publisher = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    Developer = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                    ReleaseDate = reader.IsDBNull(9)
                        ? null : DateOnly.FromDateTime(reader.GetDateTime(9)),
                    FreeToGameProfileUrl = reader.IsDBNull(10)
                        ? string.Empty  : reader.GetString(10)
                };
                return game;
            }
            
            return null;

        }



        public async Task InsertAsync(Game game)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
            IF NOT EXISTS (SELECT 1 FROM Games WHERE Id = @Id)
            BEGIN
                INSERT INTO Games 
                (Id, Title, Thumbnail, ShortDescription, GameUrl, Genre, Platform, Publisher, Developer, ReleaseDate, FreeToGameProfileUrl)
                VALUES 
                (@Id, @Title, @Thumbnail, @ShortDescription, @GameUrl, @Genre, @Platform, @Publisher, @Developer, @ReleaseDate, @FreeToGameProfileUrl)
            END";

            using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@Id", SqlDbType.Int).Value = game.Id;
            command.Parameters.Add("@Title", SqlDbType.NVarChar, 200).Value = game.Title ?? string.Empty;
            command.Parameters.Add("@Thumbnail", SqlDbType.NVarChar, 500).Value = game.Thumbnail ?? string.Empty;
            command.Parameters.Add("@ShortDescription", SqlDbType.NVarChar, -1).Value = game.ShortDescription ?? string.Empty;
            command.Parameters.Add("@GameUrl", SqlDbType.NVarChar, 500).Value = game.GameUrl ?? string.Empty;
            command.Parameters.Add("@Genre", SqlDbType.NVarChar, 100).Value = game.Genre ?? string.Empty;
            command.Parameters.Add("@Platform", SqlDbType.NVarChar, 100).Value = game.Platform ?? string.Empty;
            command.Parameters.Add("@Publisher", SqlDbType.NVarChar, 150).Value = game.Publisher ?? string.Empty;
            command.Parameters.Add("@Developer", SqlDbType.NVarChar, 150).Value = game.Developer ?? string.Empty;
            command.Parameters.Add("@FreeToGameProfileUrl", SqlDbType.NVarChar, 500).Value = game.FreeToGameProfileUrl ?? string.Empty;
            command.Parameters.Add("@ReleaseDate", SqlDbType.Date).Value = game.ReleaseDate.HasValue
                    ? game.ReleaseDate.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value;

            await command.ExecuteNonQueryAsync();
        }



        public async Task InsertManyAsync(IEnumerable<Game> games)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
            IF NOT EXISTS (SELECT 1 FROM Games WHERE Id = @Id)
            BEGIN
                INSERT INTO Games 
                (Id, Title, Thumbnail, ShortDescription, GameUrl, Genre, Platform, Publisher, Developer, ReleaseDate, FreeToGameProfileUrl)
                VALUES 
                (@Id, @Title, @Thumbnail, @ShortDescription, @GameUrl, @Genre, @Platform, @Publisher, @Developer, @ReleaseDate, @FreeToGameProfileUrl)
            END";

            using var transcation = connection.BeginTransaction();

            using var command = new SqlCommand(sql, connection, transcation);

            command.Parameters.Add("@Id", SqlDbType.Int);
            command.Parameters.Add("@Title", SqlDbType.NVarChar, 200);
            command.Parameters.Add("@Thumbnail", SqlDbType.NVarChar, 500);
            command.Parameters.Add("@ShortDescription", SqlDbType.NVarChar, -1);
            command.Parameters.Add("@GameUrl", SqlDbType.NVarChar, 500);
            command.Parameters.Add("@Genre", SqlDbType.NVarChar, 100);
            command.Parameters.Add("@Platform", SqlDbType.NVarChar, 100);
            command.Parameters.Add("@Publisher", SqlDbType.NVarChar, 150);
            command.Parameters.Add("@Developer", SqlDbType.NVarChar, 150);
            command.Parameters.Add("@FreeToGameProfileUrl", SqlDbType.NVarChar, 500);
            command.Parameters.Add("@ReleaseDate", SqlDbType.Date);

            try
            {
                foreach (var game in games)
                {
                    command.Parameters["@Id"].Value = game.Id;
                    command.Parameters["@Title"].Value = game.Title ?? string.Empty;
                    command.Parameters["@Thumbnail"].Value = game.Thumbnail ?? string.Empty;
                    command.Parameters["@ShortDescription"].Value = game.ShortDescription ?? string.Empty;
                    command.Parameters["@GameUrl"].Value = game.GameUrl ?? string.Empty;
                    command.Parameters["@Genre"].Value = game.Genre ?? string.Empty;
                    command.Parameters["@Platform"].Value = game.Platform ?? string.Empty;
                    command.Parameters["@Publisher"].Value = game.Publisher ?? string.Empty;
                    command.Parameters["@Developer"].Value = game.Developer ?? string.Empty;
                    command.Parameters["@FreeToGameProfileUrl"].Value = game.FreeToGameProfileUrl ?? string.Empty;
                    command.Parameters["@ReleaseDate"].Value = game.ReleaseDate.HasValue
                        ? game.ReleaseDate.Value.ToDateTime(TimeOnly.MinValue)
                        : DBNull.Value;

                    await command.ExecuteNonQueryAsync();
                }
                await transcation.CommitAsync();
            }
            catch
            {
                await transcation.RollbackAsync();
                throw;
            }

        }
}
}

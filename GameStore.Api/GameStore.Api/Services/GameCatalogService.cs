using GameStore.Api.Dtos;
using GameStore.Api.Models;
using GameStore.Api.Repositories;
using System.Text.Json;
using System.Net.Http.Json;

namespace GameStore.Api.Services
{
    public class GameCatalogService : IGameCatalogService
    {
        private readonly IGameRepository _repo;
        private readonly HttpClient _client;
        private readonly ILogger<GameCatalogService> _logger;
        private const int CacheThreshold = 50;

        public GameCatalogService(IGameRepository repo, HttpClient client, ILogger<GameCatalogService> logger)
        {
            _repo = repo;
            _client = client;
            _logger = logger;
        }

        //Dto to entity mapper
        private static Game ToEntity(FreeToGameDto dto) => new()
        {
            Id = dto.Id,
            Title = dto.Title ?? string.Empty,
            Thumbnail = dto.Thumbnail ?? string.Empty,
            ShortDescription = dto.ShortDescription ?? string.Empty,
            GameUrl = dto.GameUrl ?? string.Empty,
            Genre = dto.Genre ?? string.Empty,
            Platform = dto.Platform ?? string.Empty,
            Publisher = dto.Publisher ?? string.Empty,
            Developer = dto.Developer ?? string.Empty,
            FreeToGameProfileUrl = dto.FreeToGameProfileUrl ?? string.Empty,
            ReleaseDate = DateOnly.TryParse(dto.ReleaseDate, out var d) ? d : null
        };


        public async Task<bool> TestDbAsync()
        {
            _logger.LogInformation("TestDbAsync Triggered");
            return await _repo.TestConnectionAsync();
        }


        public async Task ClearDatabaseAsync()
        {
            _logger.LogInformation("ClearDatabaseAsync Triggered");
            await _repo.ClearAllAsync();
        }


        public async Task<List<Game>> GetAllGamesAsync()
        {
            _logger.LogInformation("GetAllGamesAsync started");

            var result = await _repo.GetAllAsync();
            //Assuming DB is incomplete if records < 50
            if (result.Count >= CacheThreshold) 
            {
                _logger.LogInformation("DB HIT: returning {Count} games from database", result.Count);
                return result;
            }

            _logger.LogWarning("DB MISS: incomplete/no games in DB. Calling FreeToGame API");
            using var response = await _client.GetAsync("games?platform=pc");
            _logger.LogInformation("FreeToGame responded {StatusCode}", (int)response.StatusCode);

            response.EnsureSuccessStatusCode();

            // Deserialize
            var apiGames = await response.Content.ReadFromJsonAsync<List<FreeToGameDto>>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            _logger.LogInformation("FreeToGame returned {Count} games", apiGames?.Count ?? 0);

            if (apiGames is null || apiGames.Count == 0)
                return new List<Game>();

            var gamesToSave = apiGames.Select(ToEntity).ToList();

            await _repo.InsertManyAsync(gamesToSave);
            _logger.LogInformation("Saved {Count} games into DB", gamesToSave.Count);

            return gamesToSave;
        }


        public async Task<Game?> GetGameByIdAsync(int id)
        {
            _logger.LogInformation("GetGameByIdAsync started");

            var result = await _repo.GetByIdAsync(id);
            if(result is not null)
            {
                _logger.LogInformation("DB HIT: returning game '{Title}' from database", result.Title);
                return result;
            }

            _logger.LogWarning("DB MISS: game {GameId} not found in DB. Calling FreeToGame API", id);
            using var response = await _client.GetAsync($"game?id={id}");
            _logger.LogInformation("FreeToGame responded {StatusCode}", (int)response.StatusCode);
            if (!response.IsSuccessStatusCode)
                return null;

            var apiGame = await response.Content.ReadFromJsonAsync<FreeToGameDto>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (apiGame is null)
                return null;

            var game = ToEntity(apiGame);

            await _repo.InsertAsync(game);
            _logger.LogInformation("Saved game '{Title}' into DB", game.Title);

            return game;
        }

        public async Task<List<Game>> GetAllCachedGamesAsync()
        {
            _logger.LogInformation("GetAllCachedGamesAsync started");

            var result = await _repo.GetAllAsync();
            if (result.Count > 0)
            {
                _logger.LogInformation("DB HIT: returning {Count} games from database", result.Count);
                return result;
            }

            return [];
        }
    }
}

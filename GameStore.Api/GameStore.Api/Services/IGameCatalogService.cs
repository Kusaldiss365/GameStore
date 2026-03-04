using GameStore.Api.Dtos;
using GameStore.Api.Models;

namespace GameStore.Api.Services
{
    public interface IGameCatalogService
    {
        Task<bool> TestDbAsync();
        Task ClearDatabaseAsync();
        Task<List<Game>> GetAllGamesAsync();
        Task<List<Game>> GetAllCachedGamesAsync();
        Task<Game?> GetGameByIdAsync(int id);
    }
}

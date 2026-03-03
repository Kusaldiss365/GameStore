using GameStore.Api.Dtos;
using GameStore.Api.Models;

namespace GameStore.Api.Services
{
    public interface IGameCatalogService
    {
        Task<List<Game>> GetAllGamesAsync();
        Task<Game?> GetGameByIdAsync(int id);
    }
}

using GameStore.Api.Models;

namespace GameStore.Api.Repositories
{
    public interface IGameRepository
    {
        Task<bool> TestConnectionAsync();
        Task ClearAllAsync();
        Task<List<Game>> GetAllAsync();
        Task<Game?> GetByIdAsync(int id);
        Task InsertAsync(Game game);
        Task InsertManyAsync(IEnumerable<Game> games);
    }
}

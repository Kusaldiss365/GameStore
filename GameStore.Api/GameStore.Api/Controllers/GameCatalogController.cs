using GameStore.Api.Models;
using GameStore.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameCatalogController : ControllerBase
    {
        private readonly IGameCatalogService _service;

        public GameCatalogController(IGameCatalogService service) {
            _service = service;
        }


        [HttpGet]
        public async Task<ActionResult<List<Game>>> GetAllGames()
        {
            var allGames = await _service.GetAllGamesAsync();
            return Ok(allGames);
        }


        [HttpGet("cached")]
        public async Task<ActionResult<List<Game>>> GetAllCachedGames()
        {
            var cachedGames = await _service.GetAllCachedGamesAsync();
            return Ok(cachedGames);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<Game>> GetGameById(int id) {

            if (id <= 0) return BadRequest("Invalid ID!");

            var game = await _service.GetGameByIdAsync(id);
            if (game == null)
                return NotFound($"Game with ID {id} was not found");

            return Ok(game);
        }


        [HttpDelete("clear")]
        public async Task<IActionResult> ClearDatabase()
        {
            await _service.ClearDatabaseAsync();
            return Ok("All games deleted successfully.");
        }


        [HttpGet("test-db")]
        public async Task<IActionResult> TestDb()
        {
            var result = await _service.TestDbAsync();
            return Ok(result);
        }
    }
}

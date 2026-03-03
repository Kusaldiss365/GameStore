using GameStore.Api.Models;
using GameStore.Api.Services;
using Microsoft.AspNetCore.Components;
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
        public async Task<ActionResult<List<Game>>> GetGames()
        {
            var all_games = await _service.GetAllGamesAsync();
            return Ok(all_games);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetGameById(int id) {

            if (id <= 0)
                return BadRequest("Invalid ID!");

            var game = await _service.GetGameByIdAsync(id);
            if (game == null)
                return NotFound($"Game with ID {id} was not found");

            return Ok(game);
        }

    }
}

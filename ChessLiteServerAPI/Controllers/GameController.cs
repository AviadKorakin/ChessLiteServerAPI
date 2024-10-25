using ChessLiteServerAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

[ApiController]
[Route("api/games")]
public class GamesController : ControllerBase
{
    private readonly ChessLiteServerAPIContext _context;

    public GamesController(ChessLiteServerAPIContext context)
    {
        _context = context;
    }

    // GET: api/games?player={player} - Get Games by Player Name
    [HttpGet]
    public async Task<IActionResult> GetGamesByPlayer([FromQuery] string player)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => (u.FirstName ?? string.Empty).ToLower() == player.ToLower());

        if (user == null)
            return NotFound(new { Message = "Player not found." });

        var games = await _context.Games
            .Where(g => g.UserId == user.Id)
            .Select(g => new
            {
                g.GameId,
                g.GameDate,
                Winner = string.IsNullOrEmpty(g.Winner) ? "Black" : g.Winner,
                WinMethod = string.IsNullOrEmpty(g.WinMethod) ? "Server lost connection to the player" : g.WinMethod,
                UserColor = g.UserColor ? "White" : "Black",   
                ComputerColor = g.ComputerColor ? "White" : "Black"  
            })
            .ToListAsync();

        return Ok(games);
    }

    // GET: api/games/{id} - Get Game Details by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGameById(int id)
    {
        var game = await _context.Games
            .Include(g => g.User)  // Include user metadata if needed
            .FirstOrDefaultAsync(g => g.GameId == id);

        if (game == null)
            return NotFound(new { Message = "Game not found" });

        var gameResponse = new
        {
            game.GameId,
            game.UserId,
            game.GameDate,
            Winner = string.IsNullOrEmpty(game.Winner) ? "Black" : game.Winner,
            WinMethod = string.IsNullOrEmpty(game.WinMethod) ? "Server lost connection to the player" : game.WinMethod
        };

        return Ok(gameResponse);
    }

    // GET: api/games/{id}/steps - Get Steps for a Specific Game
    [HttpGet("{id}/steps")]
    public async Task<IActionResult> GetGameStepsByGameId(int id)
    {
        var gameSteps = await _context.GameSteps
            .Where(s => s.GameId == id)
            .OrderBy(s => s.StepOrder)  // Ensure steps are ordered correctly
            .Select(s => new
            {
                s.StepOrder,
                s.FromPosition,
                s.ToPosition,
                s.PieceType,
                s.Promotion
            })
            .ToListAsync();

        if (!gameSteps.Any())
        {
            return NotFound(new { Message = "No steps found for the specified game" });
        }

        return Ok(gameSteps);
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChessLiteServerAPI.Data;
using ChessLiteServerAPI.Models;

namespace ChessLiteServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ChessLiteServerAPIContext _context;

        public UsersController(ChessLiteServerAPIContext context)
        {
            _context = context;
        }

        // DELETE: api/Users/{userId}/Games/{gameId}
        [HttpDelete("{userId}/Games/{gameId}")]
        public async Task<IActionResult> DeleteGame(int userId, int gameId)
        {
            var user = await _context.Users
                .Include(u => u.Games)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            var game = user.Games.FirstOrDefault(g => g.GameId == gameId);
            if (game == null)
            {
                return NotFound(new { Message = "Game not found." });
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Game deleted successfully." });
        }
    }
}

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLiteServerAPI.Data;
using ChessLiteServerAPI.Models;

namespace ChessLiteServerAPI.Pages.ChessQuestion
{
    public class IndexModel : PageModel
    {
        private readonly ChessLiteServerAPIContext _context;

        public IndexModel(ChessLiteServerAPIContext context)
        {
            _context = context;
        }

        public IList<User> AllPlayers { get; set; } = new List<User>();
        public IList<PlayerGames> PlayersWithLastGameDate { get; set; } = new List<PlayerGames>();
        public IList<Game> AllGames { get; set; } = new List<Game>();
        public IList<FirstPlayerByCountry> FirstPlayersByCountry { get; set; } = new List<FirstPlayerByCountry>();
        public IList<PlayerGameCount> PlayerGameCounts { get; set; } = new List<PlayerGameCount>();
        public IList<PlayerGames> PlayersOrderedByGames { get; set; } = new List<PlayerGames>();
        public IList<string> DistinctPlayerNames { get; set; } = new List<string>();
        public IList<IGrouping<int, User>> PlayersByGameCount { get; set; } = new List<IGrouping<int, User>>();
        public IList<IGrouping<string, User>> PlayersByCountry { get; set; } = new List<IGrouping<string, User>>();

        public async Task OnGetAsync()
        {
            // Query 1: Retrieve all players, sorted alphabetically by FirstName
            AllPlayers = await _context.Users
                .OrderBy(u => (u.FirstName ?? string.Empty).ToLower()) // Handle null values with empty string
                .ToListAsync();

            // Query 2: Retrieve players with their last game date, sorted by date and name in descending order
            PlayersWithLastGameDate = await _context.Users
                .Select(u => new PlayerGames
                {
                    Name = u.FirstName, // Player's first name
                    LastGameDate = _context.Games // Query games related to this user
                        .Where(g => g.UserId == u.Id) // Filter by UserId
                        .OrderByDescending(g => g.GameDate) // Sort by GameDate in descending order
                        .Select(g => (DateTime?)g.GameDate) // Select the date as a nullable value
                        .FirstOrDefault() // Take the latest date or null if no games found
                })
                .OrderByDescending(pg => (pg.Name ?? string.Empty).ToLower()) // Sort by name in descending orde
                .ThenByDescending(pg => pg.LastGameDate.HasValue) // then by date if exists
                .ToListAsync();

            // Query 3: Retrieve all games with their steps included
            AllGames = await _context.Games
                .Include(g => g.GameSteps) // Include related GameSteps
                .ToListAsync();

            // Query 4: Retrieve the first player to ever play a game from each country, sorted alphabetically by country
            FirstPlayersByCountry = await _context.Users
                .Where(u => !string.IsNullOrEmpty(u.Country)) // Filter users with non-empty country field
                .Select(u => new
                {
                    User = u,
                    FirstGameDate = _context.Games // Get the earliest game date for each user
                        .Where(g => g.UserId == u.Id)
                        .OrderBy(g => g.GameDate)
                        .Select(g => (DateTime?)g.GameDate)
                        .FirstOrDefault() // Nullable DateTime to handle users with no games
                })
                .Where(x => x.FirstGameDate.HasValue) // Keep only users with games
                .GroupBy(x => x.User.Country!) // Group by country (non-nullable after filter)
                .Select(g => new FirstPlayerByCountry
                {
                    Country = g.Key, // Group key (country)
                    FirstPlayer = g.OrderBy(x => x.FirstGameDate).First().User // Get the first player by earliest game date
                })
                .OrderBy(fp => fp.Country) // Sort alphabetically by country
                .ToListAsync();

            // Query 5: Retrieve distinct player names for use in a combo box, sorted alphabetically
            DistinctPlayerNames = await _context.Users
                .Select(u => (u.FirstName ?? string.Empty).ToLower()) // Handle null values and convert to lowercase
                .Distinct() // Select distinct names only
                .OrderBy(name => name) // Sort alphabetically
                .ToListAsync();

            // Query 6: Retrieve the count of games played by each player
            PlayerGameCounts = await _context.Users
                .Select(u => new PlayerGameCount
                {
                    PlayerName = u.FirstName, // Player's name
                    GameCount = _context.Games.Count(g => g.UserId == u.Id) // Count games for this user
                })
                .ToListAsync();

            // Query 7: Fetch all users from the database (for grouping by game count and country)
            var users = await _context.Users
                .Where(u => !string.IsNullOrEmpty(u.Country)) // Filter users with a valid country
                .ToListAsync();

            // Query 8: Retrieve players with their game counts and group them by the game count
            var playersWithGames = await _context.Users
                .Select(u => new
                {
                    Player = u, // Player entity
                    GameCount = _context.Games.Count(g => g.UserId == u.Id) // Count games for the player
                })
                .ToListAsync();

            // Group players by the number of games they played, sorted by game count in descending order
            PlayersByGameCount = playersWithGames
                .GroupBy(p => p.GameCount, p => p.Player) // Group by game count
                .OrderByDescending(g => g.Key) // Sort groups by game count (descending)
                .ToList();

            // Query 9: Group users by country, sorted alphabetically by country
            PlayersByCountry = users
                .Where(u => u.Country != null) // Ensure country is not null
                .GroupBy(u => u.Country!) // Group by country (using null-forgiving operator)
                .OrderBy(g => g.Key) // Sort groups by country name
                .ToList();
        }
        public int? GetUserIdByName(string playerName)
        {
            var user = _context.Users
                .FirstOrDefault(u => (u.FirstName ?? string.Empty).ToLower() == playerName.ToLower());
            return user?.Id;
        }
    }

    // Supporting classes for structured data
    public class PlayerGames
    {
        public string? Name { get; set; } // Player's name (nullable)
        public DateTime? LastGameDate { get; set; } // Last game date (nullable)
        public int GameCount { get; set; } // Total game count
    }

    public class FirstPlayerByCountry
    {
        public string? Country { get; set; } // Country name (nullable)
        public User? FirstPlayer { get; set; } // First player from this country (nullable)
    }

    public class PlayerGameCount
    {
        public string? PlayerName { get; set; } // Player's name (nullable)
        public int GameCount { get; set; } // Number of games played
    }
}

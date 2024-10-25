using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLiteServerAPI.Data;
using ChessLiteServerAPI.Models;

namespace ChessLiteServerAPI.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly ChessLiteServerAPIContext _context;

        public IndexModel(ChessLiteServerAPIContext context)
        {
            _context = context;
        }

        public IList<User> Users { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Users = await _context.Users
                .Include(u => u.Games) // Eager load the games
                .ToListAsync();
        }

       
    }
}

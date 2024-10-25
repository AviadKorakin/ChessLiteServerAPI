using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ChessLiteServerAPI.Data;
using ChessLiteServerAPI.Models;

namespace ChessLiteServerAPI.Pages.Users
{
    public class DeleteModel : PageModel
    {
        private readonly ChessLiteServerAPI.Data.ChessLiteServerAPIContext _context;

        public DeleteModel(ChessLiteServerAPI.Data.ChessLiteServerAPIContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User Users { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                Users = user;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                Users = user;
                _context.Users.Remove(Users);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}

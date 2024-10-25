using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ChessLiteServerAPI.Models
{
    [Table("Users")]  // Specify the table name in your database
    public class User
    {
        [Required(ErrorMessage = "Identifier is required")]
        [Range(1, 1000, ErrorMessage = "Identifier must be between 1 and 1000")]
        public int Id { get; set; }

        [Display(Name = "First Name")]  // This will display "First Name"
        [Required(ErrorMessage = "First name is required")]
        [RegularExpression(@"^[A-Za-z]{2,50}$", ErrorMessage = "First name must be between 2 and 50 characters long and contain only letters without spaces or numbers")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
        [StringLength(10, ErrorMessage = "Phone number must be exactly 10 digits", MinimumLength = 10)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(50, ErrorMessage = "Country name cannot be longer than 50 characters")]
        public string? Country { get; set; }

        // Navigation property to establish relationship with Game
        public virtual ICollection<Game> Games { get; set; } = new List<Game>(); // One User can have many Games
    }
}

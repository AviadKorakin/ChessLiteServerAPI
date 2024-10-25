using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChessLiteServerAPI.Models
{
    [Table("Games")]  // Specify the table name in your database
    public class Game
    {
        [Key]  // Mark as primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Identity column
        public int GameId { get; set; }

        [Required] // UserId is required
        public int UserId { get; set; }

        [Required] // GameDate is required
        public DateTime GameDate { get; set; }

        public string? Winner { get; set; }
        public string? WinMethod { get; set; }

        [Required] // Indicate if the computer is playing with white
        public bool ComputerColor { get; set; }

        [Required] // Indicate if the user is playing with white
        public bool UserColor { get; set; }

        // Navigation property
        [ForeignKey("UserId")] // Specify the foreign key
        public virtual User User { get; set; } = default!;

        public virtual ICollection<GameStep> GameSteps { get; set; } = new List<GameStep>();
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChessLiteServerAPI.Models
{
    [Table("GameSteps")]
    public class GameStep
    {
        [Key] // This marks the property as the primary key
        public int StepId { get; set; }  // Primary Key

        public int GameId { get; set; }  // Foreign key to the Game

        public int StepOrder { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public string FromPosition { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public string ToPosition { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public string PieceType { get; set; } = string.Empty;

        public string? Promotion { get; set; } // Nullable field for promotions

        // New boolean fields for en passant and castling
        [Required]
        public bool EnPassant { get; set; } = false;

        [Required]
        public bool Castling { get; set; } = false;

        // Navigation property 
        public Game Game { get; set; } = default!;
    }
}

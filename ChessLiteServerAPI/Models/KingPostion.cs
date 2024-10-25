using System.Text.Json.Serialization;

namespace ChessLiteServerAPI.Models
{
    public class KingPosition
    {
        public int Row { get; set; }
        public int Col { get; set; }

        // Parameterless constructor for deserialization
        public KingPosition() { }

        public KingPosition(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }
}

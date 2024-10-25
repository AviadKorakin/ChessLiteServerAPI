namespace ChessLiteServerAPI.Models
{
    public class UpdateWinnerRequest
    {
        public int GameId { get; set; }
        public string Winner { get; set; } = string.Empty;
        public string WinMethod { get; set; } = string.Empty;
    }
}
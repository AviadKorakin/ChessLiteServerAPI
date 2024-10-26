using ChessLiteServerAPI.Exceptions;
using ChessLiteServerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ChessLiteServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChessController : ControllerBase
    {
        private static readonly string[] Promotions = { "Knight", "Bishop", "Rook" };
        private static readonly Random Random = new Random();
        private readonly GameRecordService _gameRecordService;
        private readonly TaskManager _taskManager;
        public ChessController(GameRecordService gameRecordService, TaskManager taskManager)
        {
            _gameRecordService = gameRecordService;
            _taskManager = taskManager;
        }

        // POST: api/chess/move
        [HttpPost("move")]
        public ActionResult<MoveResponse> GetRandomLegalMove([FromBody] Chessboard board)
        {
            if (board == null)
            {
                return BadRequest("Invalid input: Chessboard data is required.");
            }


            try
            {
                var allValidMoves = new List<((int Row, int Col) From, (int Row, int Col) To, ChessPiece Piece)>();

                // Collect all valid moves for the current player's pieces
                for (int row = 0; row < board.Board.Length; row++)
                {
                    for (int col = 0; col < board.Board[row].Length; col++)
                    {
                        var piece = board.Board[row][col];

                        if (piece != null && piece.Color == board.CurrentTurn)
                        {
                            var moves = piece.Range((row, col), board.Board);

                            // Add each valid move to the list along with its originating piece
                            foreach (var move in moves)
                            {
                                allValidMoves.Add(((row, col), move, piece));
                            }
                        }
                    }
                }

                // Shuffle the collected moves
                allValidMoves = allValidMoves.OrderBy(_ => Random.Next()).ToList();

                // Iterate through the shuffled moves and attempt to make them
                foreach (var (from, to, piece) in allValidMoves)
                {
                    try
                    {
                        bool success = board.MovePiece(from, to);

                        if (success)
                        {
                            // Log the move made to the console
                            Console.WriteLine($"Move made: {piece.Type} from ({from.Row}, {from.Col}) to ({to.Row}, {to.Col})");
                            return Ok(MoveResponse.ValidMove(from, to));
                        }
                    }
                    catch (PawnException)
                    {
                        string promotion = Promotions[Random.Next(Promotions.Length)];
                        return Ok(MoveResponse.PawnPromotion(from, to, promotion));
                    }
                    catch (IlegalMoveException)
                    {
                        Console.WriteLine($"Illegal move attempted: ({from.Row}, {from.Col}) -> ({to.Row}, {to.Col})");
                        continue; // Continue to the next move
                    }
                    catch (CheckmateException)
                    {

                        // Log the move made to the console
                        Console.WriteLine("Checkmate");
                        return Ok(MoveResponse.ValidMove(from, to));
                    }
                    catch (CastlingException)
                    {

                        // Log the move made to the console
                        Console.WriteLine("Castling!");
                        return Ok(MoveResponse.ValidMove(from, to));
                    }
                    catch (EnpassantException)
                    {
                        Console.WriteLine($"Move made: {piece.Type} from ({from.Row}, {from.Col}) to ({to.Row}, {to.Col})");
                        // Log the move made to the console
                        Console.WriteLine("En-passant");
                        return Ok(MoveResponse.ValidMove(from, to));
                    }
                    catch (TieException)
                    {
                        // Log the move made to the console
                        Console.WriteLine("Tie");
                        return Ok(MoveResponse.ValidMove(from, to));
                    }
                    catch (InvalidOperationException)
                    {
                        // Handle InvalidOperationException specifically
                        return BadRequest("Invalid move attempted due to game state restrictions.");
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, MoveResponse.ErrorResponse($"Unexpected error: {ex.Message}"));
                    }
                }

                return BadRequest(MoveResponse.ErrorResponse("No legal moves available."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, MoveResponse.ErrorResponse($"Internal server error: {ex.Message}"));
            }
        }

        // POST: api/chess/move
        [HttpPost("move-store")]
        public ActionResult<MoveResponse> GetRandomLegalMoveAndSave([FromBody] CombinedMoveRequest comboinedMove)
        {
            if (comboinedMove.Board == null)
            {
                return BadRequest("Invalid input: Chessboard data is required.");
            }


            try
            {
                var allValidMoves = new List<((int Row, int Col) From, (int Row, int Col) To, ChessPiece Piece)>();

                // Collect all valid moves for the current player's pieces
                for (int row = 0; row < comboinedMove.Board.Board.Length; row++)
                {
                    for (int col = 0; col < comboinedMove.Board.Board[row].Length; col++)
                    {
                        var piece = comboinedMove.Board.Board[row][col];

                        if (piece != null && piece.Color == comboinedMove.Board.CurrentTurn)
                        {
                            var moves = piece.Range((row, col), comboinedMove.Board.Board);

                            // Add each valid move to the list along with its originating piece
                            foreach (var move in moves)
                            {
                                allValidMoves.Add(((row, col), move, piece));
                            }
                        }
                    }
                }

                // Shuffle the collected moves
                allValidMoves = allValidMoves.OrderBy(_ => Random.Next()).ToList();

                // Iterate through the shuffled moves and attempt to make them
                foreach (var (from, to, piece) in allValidMoves)
                {
                    try
                    {
                        bool success = comboinedMove.Board.MovePiece(from, to);

                        if (success)
                        {
                            comboinedMove.StepOrder++;
                            // Log the move made to the console
                            Console.WriteLine($"Move made: {piece.Type} from ({from.Row}, {from.Col}) to ({to.Row}, {to.Col})");
                            var task = Task.Run(() =>
                            {
                                _gameRecordService.SaveStep(
                                                        comboinedMove.GameId,
                                                             from, to,
                                                        comboinedMove.StepOrder,
                                                            piece.Type,
                                                                null,
                                                                false,
                                                                false
                                                        );
                            });
                            // Register the task in the TaskManager
                            _taskManager.AddTask(task);

                            return Ok(MoveResponse.ValidMove(from, to));
                        }
                    }
                    catch (PawnException)
                    {
                        comboinedMove.StepOrder++;
                        string promotion = Promotions[Random.Next(Promotions.Length)];
                        var task = Task.Run(() =>
                        {
                            _gameRecordService.SaveStep(
                                                    comboinedMove.GameId,
                                                         from, to,
                                                    comboinedMove.StepOrder,
                                                        piece.Type,
                                                            promotion,
                                                            false,
                                                            false
                                                    );
                        });
                        // Register the task in the TaskManager
                        _taskManager.AddTask(task);
                        return Ok(MoveResponse.PawnPromotion(from, to, promotion));
                    }
                    catch (IlegalMoveException)
                    {
                        Console.WriteLine($"Illegal move attempted: ({from.Row}, {from.Col}) -> ({to.Row}, {to.Col})");
                        continue; // Continue to the next move
                    }
                    catch (CheckmateException)
                    {

                        // Log the move made to the console
                        Console.WriteLine("Checkmate");
                        return Ok(MoveResponse.ValidMove(from, to));
                    }
                    catch (CastlingException)
                    {
                        comboinedMove.StepOrder++;
                        // Log the move made to the console
                        Console.WriteLine("Castling!");
                        var task = Task.Run(() =>
                        {
                            _gameRecordService.SaveStep(
                                                    comboinedMove.GameId,
                                                         from, to,
                                                    comboinedMove.StepOrder,
                                                        piece.Type,
                                                            null,
                                                            false,
                                                            true
                                                    );
                        });
                        // Register the task in the TaskManager
                        _taskManager.AddTask(task);
                        return Ok(MoveResponse.ValidMove(from, to));
                    }
                    catch (EnpassantException)
                    {
                        comboinedMove.StepOrder++;
                        Console.WriteLine($"Move made: {piece.Type} from ({from.Row}, {from.Col}) to ({to.Row}, {to.Col})");
                        // Log the move made to the console
                        Console.WriteLine("En-passant");
                        var task = Task.Run(() =>
                        {
                            _gameRecordService.SaveStep(
                                                    comboinedMove.GameId,
                                                         from, to,
                                                    comboinedMove.StepOrder,
                                                        piece.Type,
                                                            null,
                                                            true,
                                                            false
                                                    );
                        });
                        // Register the task in the TaskManager
                        _taskManager.AddTask(task);
                        return Ok(MoveResponse.ValidMove(from, to));
                    }
                    catch (TieException)
                    {
                        // Log the move made to the console
                        Console.WriteLine("Tie");
                        return Ok(MoveResponse.ValidMove(from, to));
                    }
                    catch (InvalidOperationException)
                    {
                        // Handle InvalidOperationException specifically
                        return BadRequest("Invalid move attempted due to game state restrictions.");
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, MoveResponse.ErrorResponse($"Unexpected error: {ex.Message}"));
                    }
                }

                return BadRequest(MoveResponse.ErrorResponse("No legal moves available."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, MoveResponse.ErrorResponse($"Internal server error: {ex.Message}"));
            }
        }
        // POST: api/chess/move
        [HttpPost("smart-move")]
        public ActionResult<MoveResponse> GetRandomSmartLegalMove([FromBody] Chessboard board)
        {
            if (board == null)
            {
                return BadRequest("Invalid input: Chessboard data is required.");
            }

            try
            {
                var allValidMoves = new List<(int Priority, (int Row, int Col) From, (int Row, int Col) To, ChessPiece Piece)>();

                // Collect all valid moves for the current player's pieces
                for (int row = 0; row < board.Board.Length; row++)
                {
                    for (int col = 0; col < board.Board[row].Length; col++)
                    {
                        var piece = board.Board[row][col];

                        if (piece != null && piece.Color == board.CurrentTurn)
                        {
                            var moves = piece.Range((row, col), board.Board);

                            // Calculate priorities and special rules
                            foreach (var move in moves)
                            {
                                int priority = CalculatePriority(board, piece, (row, col), move);
                                allValidMoves.Add((priority, (row, col), move, piece));
                            }
                        }
                    }
                }

                // Sort moves by priority in descending order
                allValidMoves = allValidMoves
                    .OrderByDescending(move => move.Priority)
                    .ThenBy(_ => Random.Next()) // Shuffle moves with the same priority
                    .ToList();

                // Iterate through the sorted moves and attempt to make them
                foreach (var (priority, from, to, piece) in allValidMoves)
                {
                    try
                    {
                        bool success = board.MovePiece(from, to);

                        if (success)
                        {
                            Console.WriteLine($"Move made: {piece.Type} from ({from.Row}, {from.Col}) to ({to.Row}, {to.Col})");
                            return Ok(MoveResponse.ValidMove(from, to));
                        }
                    }
                    catch (PawnException)
                    {
                        string promotion = Promotions[Random.Next(Promotions.Length)];
                        return Ok(MoveResponse.PawnPromotion(from, to, promotion));
                    }
                    catch (IlegalMoveException)
                    {
                        Console.WriteLine($"Illegal move attempted: ({from.Row}, {from.Col}) -> ({to.Row}, {to.Col})");
                        continue; // Continue to the next move
                    }
                    catch (CheckmateException)
                    {
                        Console.WriteLine("Checkmate");
                        return Ok(MoveResponse.ValidMove(from, to));
                    }
                    catch (CastlingException)
                    {
                        Console.WriteLine("Castling!");
                        return Ok(MoveResponse.ValidMove(from, to));
                    }
                    catch (EnpassantException)
                    {
                        Console.WriteLine($"En-passant move: {piece.Type} from ({from.Row}, {from.Col}) to ({to.Row}, {to.Col})");
                        return Ok(MoveResponse.ValidMove(from, to));
                    }
                    catch (TieException)
                    {
                        Console.WriteLine("Tie");
                        return Ok(MoveResponse.ValidMove(from, to));
                    }
                    catch (InvalidOperationException)
                    {
                        return BadRequest("Invalid move attempted due to game state restrictions.");
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, MoveResponse.ErrorResponse($"Unexpected error: {ex.Message}"));
                    }
                }

                return BadRequest(MoveResponse.ErrorResponse("No legal moves available."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, MoveResponse.ErrorResponse($"Internal server error: {ex.Message}"));
            }
        }




        // POST: api/chess/store-game
        [HttpPost("store-game")]
        public ActionResult<int> StoreGame([FromBody] StoreGameRequest request)
        {
            if (request.UserId <= 0)
            {
                return BadRequest("Invalid input: User ID is required.");
            }

            try
            {
                // Save the game and get the generated GameId
                int gameId = _gameRecordService.SaveGame(request.UserId, request.UserColor, request.ComputerColor);

                // Log the successful game storage
                Console.WriteLine($"Game stored successfully: UserId = {request.UserId}, GameId = {gameId}");

                return Ok(gameId); // Return the GameId as a response
            }
            catch (SqlException sqlEx) // Assuming you're using SqlConnection
            {
                // Check for foreign key constraint violation (error number may vary)
                if (sqlEx.Number == 547) // SQL Server error number for foreign key violation
                {
                    return NotFound("User ID does not exist."); // Return 404 if the foreign key constraint fails
                }

                // Handle other SQL exceptions if needed
                return StatusCode(500, $"Internal server error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // POST: api/chess/store-step
        [HttpPost("store-step")]
        public ActionResult StoreSingleGameStep([FromBody] MoveStepRequest moveRecord)
        {
            if (moveRecord == null || moveRecord.GameId <= 0)
            {
                return BadRequest("Invalid input: GameId and move details are required.");
            }

            try
            {
                // Fire-and-forget: Save step asynchronously
                var task = Task.Run(() =>
                {
                    _gameRecordService.SaveStep(
                        moveRecord.GameId,
                        moveRecord.From,
                        moveRecord.To,
                        moveRecord.StepOrder,
                        moveRecord.PieceType,
                        moveRecord.Promotion,
                        moveRecord.EnPassant,
                        moveRecord.Castling
                    );

                    // Log the step to the console
                    Console.WriteLine($"Step {moveRecord.StepOrder}: ({moveRecord.From.Row}, {moveRecord.From.Col}) -> ({moveRecord.To.Row}, {moveRecord.To.Col}) stored successfully");
                });

                // Register the task in the TaskManager
                _taskManager.AddTask(task);

                return Ok("Game step stored successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/chess/store-steps
        [HttpPost("store-steps")]
        public ActionResult StoreGameStep([FromBody] List<MoveStepRequest> moveRecords)
        {
            if (moveRecords == null || !moveRecords.Any())
            {
                return BadRequest("Invalid input: At least one move record is required.");
            }

            try
            {
                // Fire-and-forget: Save steps asynchronously
                var task = Task.Run(() =>
                {
                    foreach (var move in moveRecords)
                    {
                        // Assuming each MoveStepRequest has the GameId and can be used directly.
                        _gameRecordService.SaveStep(move.GameId, move.From, move.To, move.StepOrder, move.PieceType, move.Promotion,move.EnPassant,move.Castling);
                        Console.WriteLine($"Step {move.StepOrder}: ({move.From.Row}, {move.From.Col}) -> ({move.To.Row}, {move.To.Col}) stored sucessfully");
                    }
                });

                // Register the task in the TaskManager
                _taskManager.AddTask(task);

                return Ok("Game steps stored successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // **NEW TEST ROUTE**
        // GET: api/chess/test
        [HttpGet("test")]
        public ActionResult<string> Test()
        {
            try
            {
                return Ok("API is working!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // POST: api/chess/print-board
        [HttpPost("print-board")]
        public ActionResult PrintBoard([FromBody] Chessboard board)
        {
            if (board == null)
            {
                return BadRequest("Invalid input: Chessboard data is required.");
            }

            try
            {
                // Print the received board to the console
                PrintChessboard(board);

                return Ok("Chessboard printed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/chess/update-winner
        [HttpPost("update-winner")]
        public ActionResult UpdateGameWinner([FromBody] UpdateWinnerRequest request)
        {
            if (request == null || request.GameId <= 0 || string.IsNullOrWhiteSpace(request.Winner) || string.IsNullOrWhiteSpace(request.WinMethod))
            {
                return BadRequest("Invalid input: GameId, Winner, and WinMethod are required.");
            }

            try
            {
                // Call the GameRecordService to update the winner
                _gameRecordService.UpdateGameWinner(request.GameId, request.Winner, request.WinMethod);

                // Log the successful update to the console
                Console.WriteLine($"Game winner updated successfully: GameId = {request.GameId}, Winner = {request.Winner}, WinMethod = {request.WinMethod}");

                return Ok("Game winner updated successfully.");
            }
            catch (SqlException sqlEx) // Assuming you're using SqlConnection
            {
                // Handle specific SQL exceptions if necessary
                return StatusCode(500, $"Internal server error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // **Handle unknown routes**
        // This will catch invalid requests like api/chess/move/123 and respond with a proper error message.
        [HttpGet("{*url}", Order = int.MaxValue)]
        public ActionResult HandleInvalidRoute(string url)
        {
            return NotFound($"Invalid route: '{url}' does not exist.");
        }

        // POST: api/chess/print-json 
        [HttpPost("print-json")]
        public ActionResult PrintJson([FromBody] Chessboard board)
        {
            if (board == null)
            {
                return BadRequest("Invalid input: Chessboard data is required.");
            }

            try
            {
                // Serialize the Chessboard object to JSON
                string json = JsonSerializer.Serialize(board, new JsonSerializerOptions { WriteIndented = true });

                // Print the JSON to the console
                Console.WriteLine("Received Chessboard JSON:");
                Console.WriteLine(json);

                return Ok("Chessboard printed to console successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // Helper method to calculate move priority
        private int CalculatePriority(Chessboard board, ChessPiece piece, (int Row, int Col) from, (int Row, int Col) to)
        {
            int priority = piece.Type switch
            {
                "Rook" => 3,    // Rook has high priority
                "Bishop" => 2,  // Bishop has medium priority
                "Knight" => 2,  // Knight has medium priority
                "Pawn" => 1,    // Pawn has low priority
                _ => 0
            };

            // Add a bonus if the move controls the center
            if (IsCenterSquare(to))
            {
                priority += 1;
            }

            // Add a bonus if the pawn move unlocks a rook or bishop
            if (piece.Type == "Pawn" && UnlocksKeyPiece(board, from))
            {
                priority += 2; // Increase priority if it unlocks a key piece
            }

            return priority;
        }

        // Helper method to determine if a move targets a center square
        private bool IsCenterSquare((int Row, int Col) move)
        {
            // Assuming the center squares are in rows 3-6 and columns 1-2 for a 4x8 board
            return (move.Row >= 2 && move.Row <= 5) && (move.Col >= 1 && move.Col <= 2);
        }

        // Helper method to determine if a pawn move unlocks a rook or bishop
        private bool UnlocksKeyPiece(Chessboard board, (int Row, int Col) from)
        {
            // Check if the piece behind the pawn is a rook or bishop
            if (from.Row > 0)
            {
                var pieceBehind = board.Board[from.Row - 1][from.Col];
                if (pieceBehind != null && (pieceBehind.Type == "Rook" || pieceBehind.Type == "Bishop"))
                {
                    return true; // Pawn move unlocks a rook or bishop
                }
            }
            return false;
        }

        private void PrintChessboard(Chessboard board)
        {
            Console.WriteLine("Current Chessboard State:");
            for (int row = 0; row < board.Board.Length; row++)
            {
                for (int col = 0; col < board.Board[row].Length; col++)
                {
                    var piece = board.Board[row][col];
                    string symbol = piece != null ? piece.UnicodeSymbol : ".";
                    Console.Write(symbol + " ");
                }
                Console.WriteLine(); // New line for each row
            }
            Console.WriteLine($"Current Turn: {board.CurrentTurn}");
            Console.WriteLine();
        }
    }
}

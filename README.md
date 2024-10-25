# 📋 ChessLiteServerAPI: ASP.NET Chess Game Server  

Welcome to **ChessLiteServerAPI**, an advanced chess server built with **ASP.NET Core** to showcase both technical mastery and modern web design. This project emphasizes the implementation of **chess logic for a custom game variant**, backend efficiency using **ADO.NET and Entity Framework**, and a responsive web experience. Here’s a breakdown of the features and the project architecture. 

---

## 🎮 **Game Overview: חצי שח (Half Chess)**  

This chess variant, called "חצי שח" (Half Chess), provides an exciting twist:  

- **Board**: 8 rows × 4 columns (half the width of a regular chessboard)  
- **Pieces**: Each side has a **King, Bishop, Knight, Rook**, and pawns aligned with these pieces.  
- **Rules Modifications**:  
  - Pawns can move **horizontally one square** (but cannot capture in this movement).  
  - Players must complete their moves within a **limited time**; otherwise, they forfeit the game.  
  - **En passant, castling, and promotions** are supported and tracked.  

---

## 🚀 **Project Features**  

### 🎲 **Interactive Chess Server**  
- **Play against the server**: The server evaluates valid moves and responds dynamically to the user.  
- **Custom logic** for castling, pawn promotions, en passant, and special pawn movements.  
- **Moves tracking**: Save all game steps to the database with **undo/redo support**.  

### 📦 **Backend Architecture**  
- **ADO.NET**: For direct and **efficient data access** when performing low-level queries.  
- **Entity Framework Core**: For **high-level ORM operations**, managing relationships (e.g., Games, Users, and GameSteps).  
- **Parallel Processing**: Handles **long-running tasks asynchronously** via a custom `TaskManager` to prevent blocking requests.  

### 🌐 **Web Design and UI**  
- **Razor Pages with CSS and JS** for a premium and responsive user interface.  
- **Bootstrap-powered layouts**: Provides mobile-friendly views with **dynamic game tables, player stats, and collapsible menus**.  
- **Custom CSS styling**: Modern design with **icons** for game actions (e.g., 🏰 castling, ♙ promotions).  
- **AJAX-based player tracking**: Fetch games dynamically based on user selection without reloading the page.  

### 🔧 **Game Logic Implementation**  
- **Game tracking**: Every move is saved to the database, with support for:
  - 🏰 **Castling moves** (tracked and validated).
  - 📈 **Pawn promotions** (to Knight, Bishop, or Rook).  
  - ➡️ **En passant** captures.  
- **Chess rules compliance**: The game ensures all **standard chess rules** are followed, with special logic for **stalemates and checkmates**.  
- **Custom board initialization**: Multiple predefined board setups for specific scenarios.  

---

## 💾 **Database Management**  
- **SQL Server** integration through **Entity Framework** for high-level ORM and **ADO.NET** for custom queries.  
- **Data Entities**:  
  - **Users**: Stores player information (name, phone, country).  
  - **Games**: Stores game metadata (player ID, winner, win method).  
  - **GameSteps**: Tracks every move made during the game.  

---

## 📋 **Sample API Routes**  

1. **Play a Move**  
   **POST** `/api/chess/move`  
   - Makes a move on the board and validates the legality.  
   - Responds dynamically based on the move:  
     - `PawnPromotion`: If a pawn reaches the end of the board.  
     - `EnPassant`: When an en passant capture is performed.  
     - `Castling`: Validates and performs castling.  
     - `Checkmate`: Declares the winner if the king is trapped.  

2. **Track Game Steps**  
   **POST** `/api/chess/store-steps`  
   - Stores multiple moves asynchronously.  
   - Uses a **Task Manager** to handle parallel requests.  

3. **Retrieve Games by Player**  
   **GET** `/api/games?player={playerName}`  
   - Fetches all games played by the selected player.  

4. **Print the Chessboard**  
   **POST** `/api/chess/print-board`  
   - Prints the current state of the board with **Unicode symbols**.  

---

## 🛠️ **Task Management and Performance Optimization**  
- **TaskManager**: Centralized **background task management** for non-blocking operations.  
- **Parallel ForEach**: Uses **parallel loops** to improve performance when evaluating board states.  

---

## 🎨 **UI Highlights**  

- **Dynamic Game Tables**  
  - View players, games, and game steps with **collapsible lists**.  
  - **Color-coded moves**: Differentiate between **white** and **black** moves.  

- **Real-time Game Tracking**  
  - **Interactive dropdowns** to filter players and display their games.  
  - **Animated buttons**: Shiny red buttons for viewing game steps.  

- **Custom Styling with Icons**  
  - **Chessboard pieces**: Displayed using **Unicode symbols**.  
    - Example: ♔ King, ♘ Knight, ♖ Rook, ♙ Pawn.  
  - **En passant and castling icons**:  
    - En passant: ♙→♙  
    - Castling: ♖↔♔  

---


4. **Access the Web Interface**  
   - Open your browser and navigate to:  
     [https://localhost:8000](https://localhost:8000)  

---

## 🌐 **Technologies Used**  
- **ASP.NET Core**: Web API development.  
- **Razor Pages**: For the UI with **Bootstrap** integration.  
- **SQL Server**: Database management with **Entity Framework** and **ADO.NET**.  
- **TaskManager**: Custom background task management.  
- **CSS & JS**: Modern styling and interactive frontend components.  


---

## 📧 **Contact Information**  
Built with ❤️ by **Aviad Korakin**  
Feel free to reach out: [aviad825@gmail.com](mailto:aviad825@gmail.com)  

---

This project demonstrates technical prowess in **backend development, parallel programming, and web design**. It showcases how an engaging chess game can be built with **ASP.NET Core** while ensuring **scalable performance and exceptional user experience**. 🎉

Here’s your updated README with a **special promotion icon** to highlight the pawn promotion visually:

---

# 📋 **ChessLiteServerAPI: ASP.NET Chess Game Server**

Welcome to **ChessLiteServerAPI**, an advanced chess server built with **ASP.NET Core** to showcase technical mastery and modern web design. This project emphasizes the implementation of **chess logic for a custom game variant**, backend efficiency using **ADO.NET and Entity Framework**, and a responsive web experience.

🎥 **Watch the Demo Video**:  https://www.youtube.com/watch?v=QPXw-yA3WaY
[![ChessLiteServerAPI Demo](https://img.youtube.com/vi/QPXw-yA3WaY/0.jpg)](https://www.youtube.com/watch?v=QPXw-yA3WaY)  
(*Click the thumbnail to watch the video.*)

---

## 🎮 **Game Overview: חצי שח (Half Chess)**  

This chess variant, called **"חצי שח"** (Half Chess), provides an exciting twist:  

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
- **ADO.NET**: For direct and **efficient data access** with low-level queries.  
- **Entity Framework Core**: For **ORM operations**, managing relationships (e.g., Games, Users, GameSteps).  
- **Parallel Processing**: Handles **long-running tasks asynchronously** using a custom `TaskManager` to prevent blocking requests.  

### 🌐 **Web Design and UI**  
- **Razor Pages with CSS and JS** for a premium and responsive interface.  
- **Bootstrap-powered layouts**: Mobile-friendly views with **dynamic game tables, player stats, and collapsible menus**.  
- **Custom CSS styling**: Icons for actions like 🏰 castling and ♙ promotions.  
- **AJAX-based player tracking**: Fetch games dynamically without reloading the page.  

---

## 🔧 **Game Logic Implementation**  
- **Game tracking**: Every move is saved to the database, with:
  - 🏰 **Castling moves** (tracked and validated).
  - 📈 **Pawn promotions** to Knight, Bishop, or Rook.  
  - ➡️ **En passant** captures.  
- **Chess rules compliance**: Ensures all **standard rules** with custom logic for **stalemates and checkmates**.  
- **Board initialization**: Supports multiple predefined setups.  

---

## 💾 **Database Management**  
- **SQL Server** integration with **Entity Framework** and **ADO.NET**.  
- **Entities**:  
  - **Users**: Stores player info (name, phone, country).  
  - **Games**: Stores game metadata (player ID, winner, win method).  
  - **GameSteps**: Tracks every move during the game.  

---

## 📋 **Sample API Routes**  

1. **Play a Move**  
   **POST** `/api/chess/move`  
   - Validates and executes the move.
   - Handles `PawnPromotion`, `EnPassant`, `Castling`, and `Checkmate`.

2. **Track Game Steps**  
   **POST** `/api/chess/store-steps`  
   - Stores multiple moves asynchronously with a **Task Manager**.

3. **Retrieve Games by Player**  
   **GET** `/api/games?player={playerName}`  
   - Fetches all games played by the user.  

4. **Print the Chessboard**  
   **POST** `/api/chess/print-board`  
   - Prints the current board state with **Unicode symbols**.  

---

## 🛠️ **Task Management and Performance Optimization**  
- **TaskManager**: Centralized **background task management** for non-blocking operations.  
- **Parallel ForEach**: Uses **parallel loops** to improve board evaluation performance.  

---

## 🎨 **UI Highlights**  

- **Dynamic Game Tables**  
  - View players, games, and steps with **collapsible lists**.  
  - **Color-coded moves**: Differentiates between **white** and **black** moves.  

- **Real-time Game Tracking**  
  - **Interactive dropdowns** for filtering players and displaying their games.  
  - **Animated buttons**: Stylish red buttons for viewing game steps.  

- **Custom Icons for Actions**  
  - Unicode chess piece symbols: ♔ King, ♘ Knight, ♖ Rook, ♙ Pawn.  
  - Special icons:
    - **En passant**: ♙→♙  
    - **Castling**: ♖↔♔  
    - **Promotion**: promoted to ♖ *(Pawn promoted to Rook)*  

---

## 🌐 **Technologies Used**  
- **ASP.NET Core**: Backend web API development.  
- **Razor Pages**: UI with **Bootstrap** integration.  
- **SQL Server**: Database management with **Entity Framework** and **ADO.NET**.  
- **TaskManager**: Custom background task management.  
- **CSS & JS**: Modern frontend components.

---

## 🖥️ **Access the Web Interface**  
1. Clone the repository and navigate to the project folder.
2. Run the application via Visual Studio or the command line.
3. Open your browser and go to:  
   [https://localhost:8000](https://localhost:8000)

---

## 📧 **Contact Information**  
Built with ❤️ by **Aviad Korakin**  
📧 [aviad825@gmail.com](mailto:aviad825@gmail.com)  

---

This project demonstrates technical prowess in **backend development, parallel programming, and web design**. It showcases how an engaging chess game can be built with **ASP.NET Core**, ensuring **scalable performance and exceptional user experience**. 🎉  

---

This version includes the **promotion icon** to enhance clarity and visualization of the pawn promotion feature. Let me know if you need further adjustments!

using System;
public class Game
{
    private const int WindowWidth = 50; 
    private const int WindowHeight = 40;
    public Game	()
	{
        Player1 = new Player();
        Player2 = new Player();
        IsRunning = true;
        Console.SetWindowSize(WindowWidth, WindowHeight);
        SystemMessage = "Place your ships!";
    }

	public Player Player1 { get; set; }
	public Player Player2 { get; set; }
	public bool IsRunning { get; set; }
    public int CursorX { get; set; } = 0;
    public int CursorY { get; set; } = 0;
    public ActiveBoard CurrentActiveBoard { get; set; } = ActiveBoard.PlayingBoard;
    private ShipOrientation CurrentShipOrientation { get; set; } = ShipOrientation.Horizontal;
    public string SystemMessage { get; set; }
    private static readonly Random random = new Random();
    public void Run()
	{
		Draw();
		do
		{
			Update();

		} while (IsRunning);
	}

	public void Update() 
	{
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:
                if (CursorY > 0) CursorY--;
                break;
            case ConsoleKey.DownArrow:
                if (CursorY < 9) CursorY++;
                break;
            case ConsoleKey.LeftArrow:
                if (CursorX > 0) CursorX--;
                break;
            case ConsoleKey.RightArrow:
                if (CursorX < 9) CursorX++;
                break;
            case ConsoleKey.R:
                if (CurrentActiveBoard == ActiveBoard.PlayingBoard)
                {
                    CurrentShipOrientation = (CurrentShipOrientation == ShipOrientation.Horizontal)
                        ? ShipOrientation.Vertical
                        : ShipOrientation.Horizontal;
                }
                break;
            case ConsoleKey.Enter:
                if (CurrentActiveBoard == ActiveBoard.PlayingBoard && Player1.ShipsToPlace.Count > 0)
                {
                    var type = Player1.ShipsToPlace.Peek();
                    var ship = new Ship
                    {
                        StartPosition = new Position(CursorX, CursorY),
                        Orientation = CurrentShipOrientation,
                        Type = type
                    };
                    Player1.Ships.Add(ship);
                    if (Player1.CanPlaceShip(Player1.PlayingBoard, ship))
                    {
                        Player1.PlaceShip(Player1.PlayingBoard, ship);
                        Player1.ShipsToPlace.Dequeue();
                    }
                }
                if (CurrentActiveBoard == ActiveBoard.TrackingBoard)
                {
                    bool isMiss;
                    bool isValidShot = Player1.Shoot(Player2, CursorX, CursorY, out isMiss);
                    if (!isValidShot && !isMiss)
                    {
                        SystemMessage = "You have shot here already!";
                        break;
                    }
                    if (isValidShot)
                    {
                        Ship hitShip = Player2.GetShipAtPosition(CursorX, CursorY);
                        if (hitShip != null && hitShip.IsSunk(Player2.PlayingBoard))
                        {
                            SystemMessage = $"You sunk the enemy {hitShip.Type}!";
                            Player2.Ships.Remove(hitShip);
                        }
                        else
                        {
                            SystemMessage = "Hit!";
                        }
                        if (Player2.AllShipsSunk())
                        {
                            SystemMessage = "You Win!";
                            IsRunning = false;
                            break;
                        }
                    }
                    else
                    {
                        SystemMessage = "Miss!";
                    }
                    Draw();
                    Thread.Sleep(800);
                    bool isValidAiShot = false;
                    int x = 0;
                    int y = 0;
                    for (int attempts = 0; attempts < 100; attempts++)
                    {
                        x = random.Next(0, 10);
                        y = random.Next(0, 10);

                        //Skip
                        if (Player2.TrackingBoard[y, x] == 'X' || Player2.TrackingBoard[y, x] == 'O')
                        {
                            continue;
                        }
                        isValidAiShot = Player2.Shoot(Player1, x, y, out isMiss);
                        break;
                    }
                    
                    if (isValidAiShot)
                    {
                        Ship hitShip = Player1.GetShipAtPosition(x, y);
                        if (hitShip != null && hitShip.IsSunk(Player1.PlayingBoard))
                        {
                            SystemMessage = $"Enemy sunk your {hitShip.Type}!";
                            Player1.Ships.Remove(hitShip);
                        } 
                        else
                        {
                            SystemMessage = "One of your ships was hit!";
                        }
                        if (Player1.AllShipsSunk())
                        {
                            SystemMessage = "You Lost!";
                            break;
                        }
                    }
                    else
                    {
                        SystemMessage = "Enemy missed!";
                    }
                }
                if (Player1.ShipsToPlace.Count == 0 && Player2.ShipsToPlace.Count > 0)
                {
                    CurrentActiveBoard = ActiveBoard.TrackingBoard;
                    Player2.PlaceAllShipsRandomly();
                    SystemMessage = "Begin Battle!";
                    CursorX = 0;
                    CursorY = 0;
                }
                break;
            case ConsoleKey.Escape:
                IsRunning = false;
                return;
        }
        Draw();
    }

	public void Draw()
	{
        Console.Clear();
        Console.WriteLine("    === BATTLESHIP vs AI ===\n");
        Console.WriteLine($"      {SystemMessage}");

        //Draw the TrackingBoard
        Console.WriteLine("Intel " + (CurrentActiveBoard == ActiveBoard.TrackingBoard ? "<- Shoot!" : ""));
        Console.WriteLine("   A  B  C  D  E  F  G  H  I  J");

        for (int row = 0; row < 10; row++)
        {
            Console.Write($"{row} ");
            for (int col = 0; col < 10; col++)
            {
                char cell = Player1.TrackingBoard[row, col];
                bool isCursorPosition = (row == CursorY && col == CursorX && CurrentActiveBoard == ActiveBoard.TrackingBoard);
                if (cell == 'X')
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (cell == 'O')
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                if (isCursorPosition)
                {
                    Console.BackgroundColor = ConsoleColor.Cyan;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.Write($" {Player1.TrackingBoard[row, col]} ");
                Console.ResetColor();
                
            }
            Console.WriteLine();
        }

        Console.WriteLine("\n");

        //Draw the PlayingBoard
        Console.WriteLine("Ships " + (CurrentActiveBoard == ActiveBoard.PlayingBoard ? "<- Place Your Ships!" : ""));
        Console.WriteLine("   A  B  C  D  E  F  G  H  I  J");

        for (int row = 0; row < 10; row++)
        {
            Console.Write($"{row} ");
            for (int col = 0; col < 10; col++)
            {
                char cell = Player1.PlayingBoard[row, col];
                bool isCursorPosition = (row == CursorY && col == CursorX && CurrentActiveBoard == ActiveBoard.PlayingBoard);
                if (cell == 'S') 
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                if (cell == 'X')
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (cell == 'O')
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                if (isCursorPosition)
                {
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                char cellChar = Player1.PlayingBoard[row, col];
                bool isPreview = false;
                bool isConflict = false;

                if (CurrentActiveBoard == ActiveBoard.PlayingBoard && Player1.ShipsToPlace.Count > 0)
                {
                    var shipType = Player1.ShipsToPlace.Peek();
                    int size = (int)shipType;

                    for (int i = 0; i < size; i++)
                    {
                        int previewX = CursorX + (CurrentShipOrientation == ShipOrientation.Horizontal ? i : 0);
                        int previewY = CursorY + (CurrentShipOrientation == ShipOrientation.Vertical ? i : 0);

                        if (previewX == col && previewY == row)
                        {
                            isPreview = true;

                            if (Player1.PlayingBoard[previewY, previewX] != '~')
                            {
                                isConflict = true;
                            }

                            break;
                        }
                    }
                }

                if (isCursorPosition && !isConflict)
                {
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else if (isPreview && isConflict)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (isPreview)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.Write($" {cellChar} ");
                Console.ResetColor();
                
            }
            Console.WriteLine();
        }

        Console.WriteLine($"\nCursor Position: {(char)('A' + CursorX)}{CursorY}");
        Console.WriteLine("\nLegend: ~ = Water, S = Ship, X = Hit, O = Miss");
        Console.WriteLine("\nUse arrow keys to move.");
        if (CurrentActiveBoard == ActiveBoard.PlayingBoard && Player1.ShipsToPlace.Count > 0)
        {
            var shipType = Player1.ShipsToPlace.Peek();
            Console.WriteLine($"Placing: {shipType} ({(int)shipType}) - {CurrentShipOrientation}");
            Console.WriteLine("Press [R] to rotate. Press [Enter] to place.");
        }
        if (CurrentActiveBoard == ActiveBoard.TrackingBoard)
        {
            Console.WriteLine("Press [Enter] to shoot.");
        }
        Console.WriteLine("\nPress [Escape] to quit anytime.");
    }
}
public enum ActiveBoard
{
    TrackingBoard, //For shots against AI
    PlayingBoard   //For player ships
}
using System;

public class Player
{
	public Player()
	{
        Ships = new List<Ship>();
        PlayingBoard = new char[10, 10];
        TrackingBoard = new char[10, 10];
        Ships = new List<Ship>();
        for (int row = 0; row < 10; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                PlayingBoard[row, col] = '~';
                TrackingBoard[row, col] = '~';
            }
        }
        ShipsToPlace.Enqueue(ShipType.Carrier);
        ShipsToPlace.Enqueue(ShipType.Battleship);
        ShipsToPlace.Enqueue(ShipType.Submarine);
        ShipsToPlace.Enqueue(ShipType.Submarine);
        ShipsToPlace.Enqueue(ShipType.Destroyer);
    }
	public List<Ship> Ships { get; set; }
    public char[,] PlayingBoard { get; set; }
    public char[,] TrackingBoard { get; set; }
    public Queue<ShipType> ShipsToPlace { get; } = new Queue<ShipType>();
    private static readonly Random random = new Random();

    public bool Shoot(Player targetPlayer, int x, int y, out bool isMiss)
    {
        if (TrackingBoard[y, x] == 'X' || TrackingBoard[y, x] == 'O')
        {
            isMiss = false;
            return false;
        }
        char targetCell = targetPlayer.PlayingBoard[y, x];
        if (targetCell == 'S')
        {
            TrackingBoard[y, x] = 'X'; // Hit
            targetPlayer.PlayingBoard[y, x] = 'X';
            isMiss = false;
            return true;
        }
        else
        {
            TrackingBoard[y, x] = 'O'; // Miss
            targetPlayer.PlayingBoard[y, x] = 'O';
            isMiss = true;
            return false;
        }
    }
    public bool CanPlaceShip(char[,] board, Ship ship)
    {
        int size = (int)ship.Type;

        for (int i = 0; i < size; i++)
        {
            int x = ship.StartPosition.X + (ship.Orientation == ShipOrientation.Horizontal ? i : 0);
            int y = ship.StartPosition.Y + (ship.Orientation == ShipOrientation.Vertical ? i : 0);

            if (x >= 10 || y >= 10 || board[y, x] != '~')
                return false;
        }

        return true;
    }
    public void PlaceShip(char[,] board, Ship ship)
    {
        int size = (int)ship.Type;

        for (int i = 0; i < size; i++)
        {
            int x = ship.StartPosition.X + (ship.Orientation == ShipOrientation.Horizontal ? i : 0);
            int y = ship.StartPosition.Y + (ship.Orientation == ShipOrientation.Vertical ? i : 0);
            ship.Positions.Add(new Position(x, y));
            board[y, x] = 'S';
        }
    }
    public void PlaceAllShipsRandomly()
    {
        while (ShipsToPlace.Count > 0)
        {
            ShipType type = ShipsToPlace.Dequeue();
            bool placed = false;

            while (!placed)
            {
                ShipOrientation orientation = (ShipOrientation)random.Next(0, 2);
                int x = random.Next(0, 10);
                int y = random.Next(0, 10);
                int length = (int)type;

                // Check for bounds and overlaps
                bool canPlace = true;
                for (int i = 0; i < length; i++)
                {
                    int checkX = x + (orientation == ShipOrientation.Horizontal ? i : 0);
                    int checkY = y + (orientation == ShipOrientation.Vertical ? i : 0);

                    if (checkX >= 10 || checkY >= 10 || PlayingBoard[checkY, checkX] != '~')
                    {
                        canPlace = false;
                        break;
                    }
                }

                if (!canPlace) continue;

                // Place the ship
                Ship ship = new Ship
                {
                    StartPosition = new Position(x, y),
                    Orientation = orientation,
                    Type = type
                };
                for (int i = 0; i < length; i++)
                {
                    int placeX = x + (orientation == ShipOrientation.Horizontal ? i : 0);
                    int placeY = y + (orientation == ShipOrientation.Vertical ? i : 0);
                    PlayingBoard[placeY, placeX] = 'S';
                    ship.Positions.Add(new Position(placeX, placeY));

                }

                Ships.Add(ship);

                placed = true;
            }
        }
    }
    public Ship GetShipAtPosition(int x, int y)
    {
        return Ships.FirstOrDefault(ship => ship.ContainsPosition(x, y));
    }
    public bool AllShipsSunk()
    {
        return Ships.All(ship => ship.IsSunk(PlayingBoard));
    }
}

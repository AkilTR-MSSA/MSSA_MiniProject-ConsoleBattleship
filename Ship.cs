using System;

public class Ship
{
    public Ship()
    {
        Positions = new List<Position>();
    }
    public Position StartPosition { get; set; }
    public ShipType Type { get; set; }
    public ShipOrientation Orientation { get; set; }
    public List<Position> Positions { get; set; }


    public bool IsSunk(char[,] board)
    {
        if (Positions == null || Positions.Count == 0)
            return false;

        // Check if all positions are hit ('X')
        foreach (var pos in Positions)
        {
            if (board[pos.Y, pos.X] != 'X')
                return false;
        }
        return true;
    }
    public bool ContainsPosition(int x, int y)
    {
        return Positions.Any(p => p.X == x && p.Y == y);
    }
}

public class Position
{
    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }
    public int X { get; set; }
    public int Y { get; set; }
}

public enum ShipType
{
    Destroyer = 2,
    Submarine = 3,
    Battleship = 4,
    Carrier = 5
}

public enum ShipOrientation
{
    Horizontal,
    Vertical
}

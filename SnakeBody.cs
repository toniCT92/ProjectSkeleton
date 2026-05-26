using Silk.NET.SDL;

namespace TheAdventure;

public class SnakeBody : GameEntity
{
    private readonly List<(int X, int Y)> _segments = new();
    private Direction _currentDirection = Direction.Right;
    private Direction? _pendingDirection;

    public SnakeBody(int startX, int startY) : base(startX, startY)
    {
        _segments.Add((startX, startY));
        _segments.Add((startX - 1, startY));
        _segments.Add((startX - 2, startY));
    }

    public IReadOnlyList<(int X, int Y)> Segments => _segments;
    public Direction CurrentDirection => _currentDirection;

    public bool TryChangeDirection(Direction next)
    {
        var isReverse = (_currentDirection, next) switch
        {
            (Direction.Up, Direction.Down) => true,
            (Direction.Down, Direction.Up) => true,
            (Direction.Left, Direction.Right) => true,
            (Direction.Right, Direction.Left) => true,
            _ => false
        };

        if (isReverse || next == _currentDirection)
        {
            return false;
        }

        _pendingDirection = next;
        return true;
    }

    public (int X, int Y) PeekNextHead()
    {
        var dir = _pendingDirection ?? _currentDirection;
        var (dx, dy) = DirectionToDelta(dir);
        var head = _segments[0];
        return (head.X + dx, head.Y + dy);
    }

    public void Move(bool grow)
    {
        if (_pendingDirection.HasValue)
        {
            _currentDirection = _pendingDirection.Value;
            _pendingDirection = null;
        }

        var (dx, dy) = DirectionToDelta(_currentDirection);
        var head = _segments[0];
        var newHead = (X: head.X + dx, Y: head.Y + dy);
        _segments.Insert(0, newHead);

        if (!grow)
        {
            _segments.RemoveAt(_segments.Count - 1);
        }

        X = newHead.X;
        Y = newHead.Y;
    }

    public bool CollidesWithSelf()
    {
        if (_segments.Count < 2)
        {
            return false;
        }
        var head = _segments[0];
        return _segments.Skip(1).Any(s => s.X == head.X && s.Y == head.Y);
    }

    public bool CollidesWithWall(int gridWidth, int gridHeight)
    {
        var head = _segments[0];
        return head.X < 0 || head.X >= gridWidth || head.Y < 0 || head.Y >= gridHeight;
    }

    public bool Contains(int x, int y)
    {
        return _segments.Any(s => s.X == x && s.Y == y);
    }

    public override void Render(Sdl sdl, IntPtr renderer, int cellSize)
    {
    }

    private static (int Dx, int Dy) DirectionToDelta(Direction dir) => dir switch
    {
        Direction.Up    => (0, -1),
        Direction.Down  => (0,  1),
        Direction.Left  => (-1, 0),
        Direction.Right => (1,  0),
        _ => (0, 0)
    };
}

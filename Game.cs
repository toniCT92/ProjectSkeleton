using Silk.NET.SDL;

namespace TheAdventure;

public sealed class Game : IDisposable
{
    private const int GridWidth = 20;
    private const int GridHeight = 20;
    private const int CellSize = 32;
    private const int MoveIntervalMs = 150;
    private const string HighScoreFile = "highscores.json";

    private readonly Sdl _sdl;
    private readonly IntPtr _window;
    private readonly IntPtr _renderer;
    private readonly SnakeBody _snake;
    private readonly Food _food;
    private readonly HighScoreStore _highScores;
    private long _lastMoveAtMs;
    private int _score;
    private GameState _state = GameState.Playing;
    private bool _gameOverHandled;
    private bool _disposed;
    private bool _quit;

    public Game()
    {
        _sdl = new Sdl(new SdlContext());

        if (_sdl.Init(Sdl.InitVideo | Sdl.InitEvents | Sdl.InitTimer) < 0)
        {
            throw new InvalidOperationException("Failed to initialize SDL.");
        }

        unsafe
        {
            _window = (IntPtr)_sdl.CreateWindow(
                "Snake",
                Sdl.WindowposUndefined,
                Sdl.WindowposUndefined,
                GridWidth * CellSize,
                GridHeight * CellSize,
                (uint)WindowFlags.AllowHighdpi
            );

            if (_window == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to create window.");
            }

            _renderer = (IntPtr)_sdl.CreateRenderer(
                (Window*)_window,
                -1,
                (uint)RendererFlags.Accelerated
            );

            if (_renderer == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to create renderer.");
            }
        }

        _snake = new SnakeBody(GridWidth / 2, GridHeight / 2);
        _food = new Food(0, 0);
        _food.Respawn(GridWidth, GridHeight, _snake);

        _highScores = new HighScoreStore(HighScoreFile);
        try
        {
            _highScores.Load();
        }
        catch (SaveCorruptedException ex)
        {
            Console.WriteLine($"Warning: could not load high scores ({ex.Message}). Starting fresh.");
        }

        PrintTopScores();

        _lastMoveAtMs = Environment.TickCount64;
    }

    public void Run()
    {
        var ev = new Event();

        while (!_quit)
        {
            while (_sdl.PollEvent(ref ev) != 0)
            {
                if (ev.Type == (uint)EventType.Quit)
                {
                    _quit = true;
                    break;
                }

                if (ev.Type == (uint)EventType.Keydown)
                {
                    var scancode = (KeyCode)ev.Key.Keysym.Scancode;

                    if (scancode == KeyCode.Escape)
                    {
                        _quit = true;
                        break;
                    }

                    if (scancode == KeyCode.R && _state == GameState.GameOver)
                    {
                        Restart();
                        continue;
                    }

                    Direction? requested = scancode switch
                    {
                        KeyCode.Up    => Direction.Up,
                        KeyCode.Down  => Direction.Down,
                        KeyCode.Left  => Direction.Left,
                        KeyCode.Right => Direction.Right,
                        _ => null
                    };

                    if (requested.HasValue && _state == GameState.Playing)
                    {
                        _snake.TryChangeDirection(requested.Value);
                    }
                }
            }

            var nowMs = Environment.TickCount64;
            if (_state == GameState.Playing && nowMs - _lastMoveAtMs >= MoveIntervalMs)
            {
                var next = _snake.PeekNextHead();
                var willEat = next.X == _food.X && next.Y == _food.Y;
                _snake.Move(grow: willEat);

                if (willEat)
                {
                    _food.Respawn(GridWidth, GridHeight, _snake);
                    _score++;
                    Console.WriteLine($"Score: {_score}");
                }

                if (_snake.CollidesWithWall(GridWidth, GridHeight) || _snake.CollidesWithSelf())
                {
                    _state = GameState.GameOver;
                }

                _lastMoveAtMs = nowMs;
            }

            if (_state == GameState.GameOver && !_gameOverHandled)
            {
                HandleGameOver();
            }

            unsafe
            {
                var r = (Renderer*)_renderer;
                _sdl.SetRenderDrawColor(r, 20, 20, 20, 255);
                _sdl.RenderClear(r);

                _food.Render(_sdl, _renderer, CellSize);
                _snake.Render(_sdl, _renderer, CellSize);

                _sdl.RenderPresent(r);
            }

            System.Threading.Thread.Sleep(16);
        }
    }

    private void HandleGameOver()
    {
        _gameOverHandled = true;
        Console.WriteLine($"Game Over! Score: {_score}");
        _highScores.AddScore(_score);
        try
        {
            _highScores.Save();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: could not save high scores ({ex.Message}).");
        }
        PrintTopScores();
        Console.WriteLine("Press R to restart, Esc to quit.");
    }

    private void Restart()
    {
        _snake.Reset(GridWidth / 2, GridHeight / 2);
        _food.Respawn(GridWidth, GridHeight, _snake);
        _score = 0;
        _state = GameState.Playing;
        _gameOverHandled = false;
        _lastMoveAtMs = Environment.TickCount64;
    }

    private void PrintTopScores()
    {
        var top = _highScores.Top5();
        if (top.Count == 0)
        {
            Console.WriteLine("Top scores: (none yet)");
            return;
        }
        Console.WriteLine("Top 5 scores:");
        var rank = 1;
        foreach (var entry in top)
        {
            Console.WriteLine($"  {rank}. {entry.Score} pts on {entry.When:yyyy-MM-dd}");
            rank++;
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        unsafe
        {
            if (_renderer != IntPtr.Zero)
            {
                _sdl.DestroyRenderer((Renderer*)_renderer);
            }
            if (_window != IntPtr.Zero)
            {
                _sdl.DestroyWindow((Window*)_window);
            }
        }

        _sdl.Quit();
        _disposed = true;
    }
}

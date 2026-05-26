using Silk.NET.SDL;

namespace TheAdventure;

public sealed class Game : IDisposable
{
    private const int GridWidth = 20;
    private const int GridHeight = 20;
    private const int CellSize = 32;
    private const int MoveIntervalMs = 150;

    private readonly Sdl _sdl;
    private readonly IntPtr _window;
    private readonly IntPtr _renderer;
    private readonly SnakeBody _snake;
    private long _lastMoveAtMs;
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
            }

            var nowMs = Environment.TickCount64;
            if (nowMs - _lastMoveAtMs >= MoveIntervalMs)
            {
                _snake.Move(false);
                Console.WriteLine($"Snake head: ({_snake.X}, {_snake.Y})");
                _lastMoveAtMs = nowMs;
            }

            unsafe
            {
                var r = (Renderer*)_renderer;
                _sdl.SetRenderDrawColor(r, 20, 20, 20, 255);
                _sdl.RenderClear(r);
                _sdl.RenderPresent(r);
            }

            System.Threading.Thread.Sleep(16);
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HadMonoGame;

public sealed class SnakeGame : Game
{
    private const int CellSize = 24;
    private const int Columns = 28;
    private const int Rows = 22;
    private const int BoardWidth = Columns * CellSize;
    private const int BoardHeight = Rows * CellSize;
    private const int HeaderHeight = 72;
    private const int WindowWidth = BoardWidth + 48;
    private const int WindowHeight = BoardHeight + HeaderHeight + 48;

    private static readonly Color Background = new(15, 23, 34);
    private static readonly Color BoardColor = new(22, 33, 45);
    private static readonly Color GridColor = new(29, 42, 55);
    private static readonly Color SnakeColor = new(91, 220, 145);
    private static readonly Color HeadColor = new(132, 245, 173);
    private static readonly Color FoodColor = new(255, 105, 111);
    private static readonly Color TextColor = new(226, 235, 240);
    private static readonly Color MutedColor = new(144, 160, 173);

    private readonly GraphicsDeviceManager _graphics;
    private readonly Random _random = new();
    private readonly List<Point> _snake = [];
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _pixel = null!;
    private SpriteFont _font = null!;
    private Point _food;
    private Direction _direction = Direction.Right;
    private Direction _nextDirection = Direction.Right;
    private KeyboardState _previousKeyboard;
    private double _moveTimer;
    private int _score;
    private bool _gameOver;
    private bool _paused;

    public SnakeGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = WindowWidth,
            PreferredBackBufferHeight = WindowHeight,
            SynchronizeWithVerticalRetrace = true
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = "Had – MonoGame";
    }

    protected override void Initialize()
    {
        ResetGame();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData([Color.White]);
        _font = Content.Load<SpriteFont>("Font");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
            return;
        }

        var keyboard = Keyboard.GetState();
        if (Pressed(keyboard, Keys.R))
        {
            ResetGame();
            _previousKeyboard = keyboard;
            base.Update(gameTime);
            return;
        }

        if (Pressed(keyboard, Keys.Space) && !_gameOver)
            _paused = !_paused;

        if (!_gameOver && !_paused)
        {
            ReadDirection(keyboard);
            _moveTimer += gameTime.ElapsedGameTime.TotalSeconds;
            var stepDuration = Math.Max(0.065, 0.14 - _score * 0.002);
            if (_moveTimer >= stepDuration)
            {
                _moveTimer -= stepDuration;
                MoveSnake();
            }
        }

        _previousKeyboard = keyboard;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Background);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        DrawText("HAD", 24, 14, 30, HeadColor);
        DrawText($"SCORE  {_score:00}", WindowWidth - 170, 21, 18, TextColor);
        DrawText("ARROWS / WASD  MOVE     SPACE  PAUSE     R  NEW GAME", 25, 51, 12, MutedColor);

        var boardX = (WindowWidth - BoardWidth) / 2;
        var boardY = HeaderHeight;
        _spriteBatch.Draw(_pixel, new Rectangle(boardX, boardY, BoardWidth, BoardHeight), BoardColor);
        for (var column = 0; column <= Columns; column++)
            _spriteBatch.Draw(_pixel, new Rectangle(boardX + column * CellSize, boardY, 1, BoardHeight), GridColor);
        for (var row = 0; row <= Rows; row++)
            _spriteBatch.Draw(_pixel, new Rectangle(boardX, boardY + row * CellSize, BoardWidth, 1), GridColor);

        DrawCell(_food, FoodColor, boardX, boardY, 5);
        for (var i = _snake.Count - 1; i >= 0; i--)
            DrawCell(_snake[i], i == 0 ? HeadColor : SnakeColor, boardX, boardY, 3);

        if (_paused || _gameOver)
        {
            _spriteBatch.Draw(_pixel, new Rectangle(boardX, boardY, BoardWidth, BoardHeight),
                new Color(7, 12, 19, 190));
            var title = _gameOver ? "GAME OVER" : "PAUSED";
            var subtitle = _gameOver ? "Press R to play again" : "Press SPACE to continue";
            DrawCentered(title, boardY + BoardHeight / 2 - 26, 28, TextColor);
            DrawCentered(subtitle, boardY + BoardHeight / 2 + 16, 16, MutedColor);
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private void DrawCell(Point cell, Color color, int boardX, int boardY, int inset)
    {
        var rectangle = new Rectangle(
            boardX + cell.X * CellSize + inset,
            boardY + cell.Y * CellSize + inset,
            CellSize - inset * 2,
            CellSize - inset * 2);
        _spriteBatch.Draw(_pixel, rectangle, color);
    }

    private void DrawText(string text, int x, int y, float scale, Color color) =>
        _spriteBatch.DrawString(_font, text, new Vector2(x, y), color, 0, Vector2.Zero, scale / 16f,
            SpriteEffects.None, 0);

    private void DrawCentered(string text, int y, float scale, Color color)
    {
        var size = _font.MeasureString(text) * (scale / 16f);
        DrawText(text, (WindowWidth - (int)size.X) / 2, y, scale, color);
    }

    private void ReadDirection(KeyboardState keyboard)
    {
        if ((Pressed(keyboard, Keys.Up) || Pressed(keyboard, Keys.W)) && _direction != Direction.Down)
            _nextDirection = Direction.Up;
        else if ((Pressed(keyboard, Keys.Down) || Pressed(keyboard, Keys.S)) && _direction != Direction.Up)
            _nextDirection = Direction.Down;
        else if ((Pressed(keyboard, Keys.Left) || Pressed(keyboard, Keys.A)) && _direction != Direction.Right)
            _nextDirection = Direction.Left;
        else if ((Pressed(keyboard, Keys.Right) || Pressed(keyboard, Keys.D)) && _direction != Direction.Left)
            _nextDirection = Direction.Right;
    }

    private bool Pressed(KeyboardState keyboard, Keys key) =>
        keyboard.IsKeyDown(key) && !_previousKeyboard.IsKeyDown(key);

    private void MoveSnake()
    {
        _direction = _nextDirection;
        var head = _snake[0] + (_direction switch
        {
            Direction.Up => new Point(0, -1),
            Direction.Down => new Point(0, 1),
            Direction.Left => new Point(-1, 0),
            _ => new Point(1, 0)
        });
        var eating = head == _food;
        var bodyToCheck = eating ? _snake : _snake.Take(_snake.Count - 1);

        if (head.X < 0 || head.X >= Columns || head.Y < 0 || head.Y >= Rows || bodyToCheck.Contains(head))
        {
            _gameOver = true;
            return;
        }

        _snake.Insert(0, head);
        if (eating)
        {
            _score++;
            PlaceFood();
        }
        else
        {
            _snake.RemoveAt(_snake.Count - 1);
        }
    }

    private void PlaceFood()
    {
        Point candidate;
        do
        {
            candidate = new Point(_random.Next(Columns), _random.Next(Rows));
        } while (_snake.Contains(candidate));

        _food = candidate;
    }

    private void ResetGame()
    {
        _snake.Clear();
        _snake.Add(new Point(Columns / 2, Rows / 2));
        _snake.Add(new Point(Columns / 2 - 1, Rows / 2));
        _snake.Add(new Point(Columns / 2 - 2, Rows / 2));
        _direction = Direction.Right;
        _nextDirection = Direction.Right;
        _moveTimer = 0;
        _score = 0;
        _gameOver = false;
        _paused = false;
        PlaceFood();
    }

    private enum Direction { Up, Down, Left, Right }
}

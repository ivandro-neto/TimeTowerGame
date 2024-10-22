using Entities.Game;
using Entities.GameStates;
using Entities.Pieces;
using Entities.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Entities.Timers;
using TimeTowerGame.Source.Delegators;
using System.Linq.Expressions;
using Entities.UI;
using System;
using System.Reflection.Metadata;
using Entities;
using Utilities.Tokens;

namespace TimeTowerGame
{
    /// <summary>
    /// Main game engine class that controls the game's initialization, state management, and rendering.
    /// </summary>
    public class Engine : Game
    {
        private User _user;
        private ApiService _apiService;
        private TokenStorage _tokenStorage;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameStateManager _gameStateManager;
        private SpriteFont _font;
        private SpriteFont _header;
        private SpriteFont _link;

        private ScoreManager _scoreManager;
        private TimerManager _timerManager;
        private HighScoreManager _highScoreManager;

        /// <summary>
        /// Event triggered when the game exits.
        /// </summary>
        public static event ExitGameEventHandler OnGameExited;

        /// <summary>
        /// Constructor that initializes the graphics, input systems, and necessary game services.
        /// </summary>
        public Engine()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 600;
            _graphics.PreferredBackBufferHeight = 700;
            _scoreManager = new ScoreManager();
            _timerManager = new TimerManager();
            _highScoreManager = new HighScoreManager("TimeTowerGame");
            OnGameExited += OnGameExit;
            _apiService = new ApiService();
            _user = new User();
            _tokenStorage = new TokenStorage();
        }

        /// <summary>
        /// Initializes the game components and applies graphics settings.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            _graphics.ApplyChanges();
        }

        /// <summary>
        /// Loads game content such as textures, fonts, and initializes game states.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Fonts/Font");
            _header = Content.Load<SpriteFont>("Fonts/Header");
            _link = Content.Load<SpriteFont>("Fonts/Link");
            _highScoreManager.LoadHighScore();

            // Load textures for game pieces
            Texture2D horusTexture = Content.Load<Texture2D>("Texture/horussium_texture");
            Texture2D fuxiTexture = Content.Load<Texture2D>("Texture/fuxi_texture");
            Texture2D xolotlTexture = Content.Load<Texture2D>("Texture/xolotl_texture");
            Texture2D nuwaTexture = Content.Load<Texture2D>("Texture/nuwa_texture");
            Texture2D Line = Content.Load<Texture2D>("Texture/line_texture");
            Texture2D chronosTexture = Content.Load<Texture2D>("Texture/Ellipse");
            Texture2D gearTexture = Content.Load<Texture2D>("Texture/gear");
            Texture2D globeTexture = Content.Load<Texture2D>("Texture/globe");
            Texture2D buttonTexture = new Texture2D(GraphicsDevice, 1, 1); // Button texture

            IPieceFactory pieceFactory = new BasicPieceFactory(xolotlTexture, nuwaTexture, fuxiTexture, horusTexture);

            // Calculate board size and position
            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;
            int columns = 8;
            int rows = 8;
            int tileSize = 32;
            int spacing = 4;

            int boardPixelWidth = columns * tileSize + (columns - 1) * spacing;
            int boardPixelHeight = rows * tileSize + (rows - 1) * spacing;
            int boardPositionX = (screenWidth - boardPixelWidth) / 2;
            int boardPositionY = (screenHeight - boardPixelHeight) / 2;

            // Create game board
            Board board = new Board(rows, columns, tileSize, pieceFactory, new Vector2(boardPositionX, boardPositionY), Line);

            // Initialize game state manager
            _gameStateManager = new GameStateManager(_scoreManager, _timerManager);

            // Add different states to the game state manager
            _gameStateManager.AddState("Menu", new MainMenuState(_apiService, _gameStateManager, _highScoreManager, buttonTexture, globeTexture, gearTexture, chronosTexture, _font, _header, _link, _user));
            _gameStateManager.AddState("GameMode", new GameModeState(_gameStateManager, buttonTexture, chronosTexture, _font, _header, _link));
            _gameStateManager.AddState("ZenMode", new ZenModeState(board));
            _gameStateManager.AddState("NormalMode", new NormalModeState(board, _font, _scoreManager));
            _gameStateManager.AddState("TimeKeeperMode", new TimeKeeperModeState(board, _font, _scoreManager, _timerManager));
            _gameStateManager.AddState("WinnerMode", new WinnerModeState(_font, _header, _scoreManager, _highScoreManager, _gameStateManager, buttonTexture));
            _gameStateManager.AddState("LoserMode", new LoseModeState(_font, _header, _scoreManager, _highScoreManager, _gameStateManager, buttonTexture));

            // Subscribe to the event for winning the game
            _gameStateManager.IsWinner += OnPlayerWin;

            // Start with the "Menu" state
            _gameStateManager.ChangeState("Menu");
        }

        /// <summary>
        /// Updates the game logic and state transitions based on user input and game conditions.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _gameStateManager.Update(gameTime, IsActive);
        }

        /// <summary>
        /// Event handler that triggers when the player wins the game.
        /// Updates high scores and changes the game state to "WinnerMode" or "LoserMode" based on the result.
        /// </summary>
        private void OnPlayerWin(object sender, WinningEventArgs e)
        {
            if (e.IsWinner && !e.IsPlaying)
            {
                // Switch to "WinnerMode" when the player wins
                if (_scoreManager.CurrentScore > _highScoreManager.HighScore)
                {
                    _user.HighScore = _scoreManager.CurrentScore;
                    _highScoreManager.SaveHighScore(_scoreManager.CurrentScore);
                }

                _gameStateManager.ChangeState("WinnerMode");
            }
            else if (!e.IsWinner && !e.IsPlaying)
            {
                // Update high score and switch to "LoserMode" if the player loses
                if (_scoreManager.CurrentScore > _highScoreManager.HighScore)
                {
                    _user.HighScore = _scoreManager.CurrentScore;
                    _highScoreManager.SaveHighScore(_scoreManager.CurrentScore);
                }

                _gameStateManager.ChangeState("LoserMode");
            }
        }

        /// <summary>
        /// Draws the game screen, rendering the current game state.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            _gameStateManager.Draw(_spriteBatch);
            base.Draw(gameTime);
        }

        /// <summary>
        /// Event handler that triggers when the game is exited.
        /// </summary>
        private void OnGameExit() => Exit();
    }
}

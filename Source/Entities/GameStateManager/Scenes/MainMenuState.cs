using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Entities.UI;
using Microsoft.Xna.Framework.Input;
using TimeTowerGame;
using System.Reflection;
using TimeTowerGame.Source.Delegators;
using System;
using System.Threading.Tasks;
using Utilities.Tokens;
using System.Reflection.Metadata;

namespace Entities.GameStates
{
    /// <summary>
    /// Represents the main menu state of the game.
    /// Handles input, UI buttons, and redirects for authentication or other scenes.
    /// </summary>
    public class MainMenuState : IGameState
    {
        private User _user;
        private ApiService _apiService;
        private GameStateManager _gameStateManager;
        private HighScoreManager _highScoreManager;
        private SpriteFont _regular;
        private SpriteFont _header;
        private SpriteFont _link;
        private TokenStorage _tokenStorage;
        private Button _leaderboardButton;
        private Button _exitButton;
        private Button _gearButton;
        private Button _globeButton;

        private Texture2D _buttonTexture;
        private Texture2D _gearTexture;
        private Texture2D _globeTexture;
        private Texture2D _chronosTexture;

        private Vector2 _highScorePosition;
        private Vector2 _titlePosition;
        private Vector2 _pressAnyKeyPosition;

        private bool _hasStarted = false;

        // Screen dimensions
        private const int ScreenWidth = 800;
        private const int ScreenHeight = 600;

        public event ExitGameEventHandler OnGameExit;

        private bool _isRedirecting;

        /// <summary>
        /// Initializes a new instance of MainMenuState with required services and assets.
        /// </summary>
        public MainMenuState(ApiService apiService, GameStateManager gameStateManager, HighScoreManager highScoreManager, Texture2D buttonTexture, Texture2D globeTexture, Texture2D gearTexture, Texture2D chronosTexture, SpriteFont regular, SpriteFont header, SpriteFont link, User user)
        {
            _gameStateManager = gameStateManager;
            _highScoreManager = highScoreManager;
            _buttonTexture = buttonTexture;
            _chronosTexture = chronosTexture;
            _globeTexture = globeTexture;
            _gearTexture = gearTexture;
            _regular = regular;
            _header = header;
            _link = link;
            _apiService = apiService;
            _isRedirecting = false;
            _tokenStorage = new TokenStorage();

            // Set positions for text elements
            _highScorePosition = new Vector2((ScreenWidth / 2) - _header.MeasureString("HIGH SCORE").X, 50);
            _titlePosition = new Vector2((ScreenWidth / 2) - _header.MeasureString("TIME TOWER").X, 180);
            _pressAnyKeyPosition = new Vector2((ScreenWidth / 2) - _link.MeasureString("PRESS ANY KEY TO START").X - 30, 400);
            _user = user;

            // Initialize buttons
            int buttonWidth = 200;
            int buttonHeight = 50;
            int buttonYStart = 300;
            int buttonSpacing = 70;

            _leaderboardButton = new Button(_buttonTexture, _regular, "LEADERBOARD", new Rectangle(0, 0, buttonWidth, buttonHeight), new Vector2((ScreenWidth / 2) - buttonWidth, buttonYStart + buttonHeight + 80), Color.Black, Color.Black, 2);


            _exitButton = new Button(_buttonTexture, _regular, "EXIT", new Rectangle(0, 0, buttonWidth, buttonHeight), new Vector2((ScreenWidth / 2) - buttonWidth, buttonYStart + 2 * buttonSpacing + buttonHeight + 80), Color.Black, Color.Black, 2);

            // Icon buttons (Gear and Globe)
            _gearButton = new Button(_gearTexture, _regular, "", new Rectangle(0, 0, 32, 32), new Vector2(520, 20), Color.White, Color.Black, 2);
            _globeButton = new Button(_globeTexture, _regular, "", new Rectangle(0, 0, 32, 32), new Vector2(50, 640), Color.White, Color.Black, 2);

            OnGameExit += ExitGame;
        }

        /// <summary>
        /// Handles scene change when a different scene is selected.
        /// </summary>
        private void OnSceneChanged(string sceneName)
        {
            _gameStateManager.ChangeState(sceneName);
        }

        /// <summary>
        /// Updates the main menu, handling button interaction and redirection logic.
        /// </summary>
        public async void Update(GameTime gameTime, bool isActive)
        {
            // Update buttons
            _leaderboardButton.Update(gameTime, isActive);
            _exitButton.Update(gameTime, isActive);
            _gearButton.Update(gameTime, isActive);
            _globeButton.Update(gameTime, isActive);

            KeyboardState keyboardState = Keyboard.GetState();

            // Start game on key press
            if (!_hasStarted && (keyboardState.GetPressedKeys().Length > 0))
            {
                _hasStarted = true;
                _gameStateManager.ChangeState("GameMode");
            }

            // Handle exit button click
            if (_exitButton.IsClicked)
            {
                ExitGame();
            }

            // Handle globe button redirect
            if (_globeButton.IsClicked && !_isRedirecting)
            {
                string token = _tokenStorage.LoadToken();

                if (string.IsNullOrEmpty(token))
                {
                    _isRedirecting = true;
                    _apiService.RedirectToLogin();
                    await _apiService.CheckAuthStatus();
                }
                else
                {
                    try
                    {
                        _user = await _apiService.GetUserProfile();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error retrieving user profile: " + ex.Message);
                    }
                }
            }
         /*    if(_leaderboardButton.IsClicked && !_isRedirecting)
            {
                _apiService.RedirectTo("https://timetower-web.vercel.app/leaderboard");
                _isRedirecting = false;
            } */
            // Process callback after redirection
            if (_apiService.IsCallbackReceived)
            {
                try
                {
                    _user = await _apiService.GetUserProfile();

                    if (_user is null)
                    {
                        Console.WriteLine("User not Found: " + _user);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error after authentication callback: " + ex.Message);
                }
                finally
                {
                    _isRedirecting = false;
                }
            }
        }

        /// <summary>
        /// Invokes the OnGameExit event.
        /// </summary>
        private void ExitGame() => OnGameExit.Invoke();

        /// <summary>
        /// Draws the main menu elements and buttons to the screen.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(_regular, $"user: {_user?.Name}", new Vector2(50, 20), Color.Black);
            spriteBatch.DrawString(_header, "HIGH SCORE", _highScorePosition, Color.Black);
            spriteBatch.DrawString(_header, _highScoreManager.HighScore.ToString(), new Vector2(_highScorePosition.X + _highScorePosition.X / 4, _highScorePosition.Y + 50), Color.Black);
            spriteBatch.DrawString(_header, "TIME TOWER", _titlePosition, Color.Black);
            spriteBatch.Draw(_chronosTexture, new Rectangle((int)(_titlePosition.X + (_titlePosition.X / 5)), (int)_titlePosition.Y + 75, 100, 100), Color.Black);

            spriteBatch.DrawString(_link, "PRESS ANY KEY TO START", _pressAnyKeyPosition, Color.Black);

            _leaderboardButton.Draw(spriteBatch);
            _exitButton.Draw(spriteBatch);

            _gearButton.Draw(spriteBatch);
            _globeButton.Draw(spriteBatch);
            spriteBatch.End();
        }

        /// <summary>
        /// Prepares the state when entering the main menu, setting up buttons and loading user profile.
        /// </summary>
        public async void Enter()
        {
            _hasStarted = false;            
            _gearButton.SceneName = "Settings";

            
            _gearButton.OnSceneChanged += OnSceneChanged;

            _highScoreManager.LoadHighScore();
            await _apiService.UpdateHighScore(_highScoreManager.HighScore);
            _user = await _apiService.GetUserProfile();
        }

        /// <summary>
        /// Cleans up when exiting the main menu state.
        /// </summary>
        public void Exit()
        {
            _gearButton.OnSceneChanged -= OnSceneChanged;
        }
    }
}

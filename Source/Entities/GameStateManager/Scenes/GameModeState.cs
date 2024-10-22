using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Entities.UI;
using Microsoft.Xna.Framework.Input;

namespace Entities.GameStates
{
    /// <summary>
    /// Represents the game mode selection state, allowing players to choose a game mode.
    /// </summary>
    public class GameModeState : IGameState
    {
        // Game state manager for managing different game states
        private GameStateManager _gameStateManager;

        // Fonts for rendering text
        private SpriteFont _regular;
        private SpriteFont _header;
        private SpriteFont _link;

        // Buttons for game modes and other interactions
        private Button _normalModeButton;
        private Button _timekeeperModeButton;
        private Button _zenModeButton;
        private Button _gearButton;
        private Button _globeButton;

        // Textures used for buttons and visual elements
        private Texture2D _buttonTexture;
        private Texture2D _gearTexture;
        private Texture2D _globeTexture;
        private Texture2D _chronosTexture;

        // Positions for text elements on the screen
        private Vector2 _highScorePosition;
        private Vector2 _titlePosition;
        private Vector2 _pressAnyKeyPosition;

        // Screen dimensions
        private const int ScreenWidth = 800;
        private const int ScreenHeight = 600;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameModeState"/> class.
        /// </summary>
        /// <param name="gameStateManager">The game state manager.</param>
        /// <param name="buttonTexture">The texture for the buttons.</param>
        /// <param name="chronosTexture">The texture for the chronos graphic.</param>
        /// <param name="regular">The font for regular text.</param>
        /// <param name="header">The font for header text.</param>
        /// <param name="link">The font for link text.</param>
        public GameModeState(GameStateManager gameStateManager, Texture2D buttonTexture, Texture2D chronosTexture, SpriteFont regular, SpriteFont header, SpriteFont link)
        {
            _gameStateManager = gameStateManager;  // Assign the game state manager
            _buttonTexture = buttonTexture;  // Assign the button texture
            _chronosTexture = chronosTexture;  // Assign the chronos texture
            
            _regular = regular;  // Assign the regular font
            _header = header;  // Assign the header font
            _link = link;  // Assign the link font

            // Set positions for text elements
            _titlePosition = new Vector2((ScreenWidth / 2) - _header.MeasureString("SELECT A GAME MODE").X + 80, 100);

            // Button dimensions
            int buttonWidth = 200;
            int buttonHeight = 50;
            int buttonYStart = 300;
            int buttonSpacing = 70;

            // Initialize buttons for game modes
            _normalModeButton = new Button(_buttonTexture, _regular, "NORMAL MODE",
                new Rectangle(0, 0, buttonWidth, buttonHeight),
                new Vector2((ScreenWidth / 2) - buttonWidth, buttonYStart + buttonHeight), Color.Black, Color.Black, 2);

            _timekeeperModeButton = new Button(_buttonTexture, _regular, "TIMEKEEPER MODE",
                new Rectangle(0, 0, buttonWidth, buttonHeight),
                new Vector2((ScreenWidth / 2) - buttonWidth, buttonYStart + buttonSpacing + buttonHeight), Color.Black, Color.Black, 2);

            _zenModeButton = new Button(_buttonTexture, _regular, "ZEN MODE",
                new Rectangle(0, 0, buttonWidth, buttonHeight),
                new Vector2((ScreenWidth / 2) - buttonWidth, buttonYStart + 2 * buttonSpacing + buttonHeight), Color.Black, Color.Black, 2);

            // Subscribe to OnSceneChanged event
        }

        /// <summary>
        /// Handles the scene change event.
        /// </summary>
        /// <param name="sceneName">The name of the scene to change to.</param>
        private void OnSceneChanged(string sceneName)
        {
            _gameStateManager.ChangeState(sceneName);  // Change the current game state
        }

        /// <summary>
        /// Updates the game mode state, checking for input and updating button states.
        /// </summary>
        /// <param name="gameTime">The time elapsed since the last update.</param>
        /// <param name="isActive">Indicates whether the game is active.</param>
        public void Update(GameTime gameTime, bool isActive)
        {
            // Update button states
            _normalModeButton.Update(gameTime, isActive);
            _timekeeperModeButton.Update(gameTime, isActive);
            _zenModeButton.Update(gameTime, isActive);
        }

        /// <summary>
        /// Draws the game mode state elements to the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for drawing.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            // Draw high score and title
            spriteBatch.DrawString(_header, "SELECT A GAME MODE", _titlePosition, Color.Black);
            spriteBatch.Draw(_chronosTexture, new Rectangle((int)(_titlePosition.X + 100), (int)_titlePosition.Y + 75, 100, 100), Color.Black);

            // Draw the buttons
            _normalModeButton.Draw(spriteBatch);
            _timekeeperModeButton.Draw(spriteBatch);
            _zenModeButton.Draw(spriteBatch);
            spriteBatch.End();
        }

        /// <summary>
        /// Called when entering the game mode state, sets up buttons and their scene names.
        /// </summary>
        public void Enter()
        {
            // Set scene names for buttons
            _normalModeButton.SceneName = "NormalMode";
            _timekeeperModeButton.SceneName = "TimeKeeperMode";
            _zenModeButton.SceneName = "ZenMode";

            // Subscribe to OnSceneChanged event
            _normalModeButton.OnSceneChanged += OnSceneChanged;
            _timekeeperModeButton.OnSceneChanged += OnSceneChanged;
            _zenModeButton.OnSceneChanged += OnSceneChanged;
        }

        /// <summary>
        /// Called when exiting the game mode state, cleans up event subscriptions.
        /// </summary>
        public void Exit()
        {
            // Unsubscribe from OnSceneChanged event
            _normalModeButton.OnSceneChanged -= OnSceneChanged;
            _timekeeperModeButton.OnSceneChanged -= OnSceneChanged;
            _zenModeButton.OnSceneChanged -= OnSceneChanged;
        }
    }
}

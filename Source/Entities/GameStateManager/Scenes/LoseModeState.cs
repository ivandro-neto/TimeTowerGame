using System;
using Entities.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeTowerGame;

namespace Entities.GameStates
{
    /// <summary>
    /// Represents the lose game mode state, managing the display of score and options to retry or return to the menu.
    /// </summary>
    public class LoseModeState : IGameState
    {
        private GameStateManager _gameStateManager;
        private HighScoreManager _highScoreManager;
        private Button _returnButton;
        private Button _retryButton;
        private ScoreManager _scoreManager;
        private SpriteFont _regular;
        private SpriteFont _header;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoseModeState"/> class.
        /// </summary>
        /// <param name="regular">The font used for regular text.</param>
        /// <param name="header">The font used for header text.</param>
        /// <param name="scoreManager">The score manager that tracks the current score.</param>
        /// <param name="highScoreManager">The high score manager that tracks high scores.</param>
        /// <param name="gameStateManager">The manager for changing game states.</param>
        /// <param name="buttonTexture">The texture used for buttons.</param>
        public LoseModeState(SpriteFont regular, SpriteFont header, ScoreManager scoreManager, HighScoreManager highScoreManager, GameStateManager gameStateManager, Texture2D buttonTexture)
        {
            _regular = regular;
            _scoreManager = scoreManager;
            _highScoreManager = highScoreManager;
            _header = header;

            Rectangle rectangle = new Rectangle(0, 0, 150, 50);
            buttonTexture.SetData(new[] { Color.White }); // Set button background color

            _gameStateManager = gameStateManager;
            _retryButton = new Button(buttonTexture, _regular, "Retry", rectangle, new Vector2(0, 0), Color.Black, Color.Black, 1);
            _returnButton = new Button(buttonTexture, _regular, "Exit", rectangle, new Vector2(0, 0), Color.Black, Color.Black, 1);
        }

        /// <summary>
        /// Draws the current lose state to the screen, including score information and buttons.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for drawing.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            string score = _scoreManager.CurrentScore.ToString();
            string highScore = _highScoreManager.HighScore.ToString();
            
            // Get the screen dimensions
            var screenWidth = spriteBatch.GraphicsDevice.Viewport.Width;
            var screenHeight = spriteBatch.GraphicsDevice.Viewport.Height;

            // Calculate positions for centering
            Vector2 titlePosition = new Vector2((screenWidth - _header.MeasureString("You Lose").X) / 2, 30);
            Vector2 scoreLabelPosition = new Vector2((screenWidth - _header.MeasureString("Your Score").X) / 2, 80);
            Vector2 scorePosition = new Vector2((screenWidth - _header.MeasureString(score).X) / 2, 140);
            Vector2 highScoreLabelPosition = new Vector2((screenWidth - _header.MeasureString("High Score").X) / 2, 200);
            Vector2 highScorePosition = new Vector2((screenWidth - _header.MeasureString(highScore).X) / 2, 260);
            
            // Update button positions dynamically
            _retryButton.Position = new Vector2((screenWidth - _retryButton.Rectangle.Width) / 2, 400);
            _returnButton.Position = new Vector2((screenWidth - _returnButton.Rectangle.Width) / 2, 460);
            
            spriteBatch.Begin();
            spriteBatch.DrawString(_header, "You Lose", titlePosition, Color.Black);
            spriteBatch.DrawString(_header, "Your Score", scoreLabelPosition, Color.Black);
            spriteBatch.DrawString(_header, score, scorePosition, Color.Black);
            spriteBatch.DrawString(_header, "High Score", highScoreLabelPosition, Color.Black);
            spriteBatch.DrawString(_header, highScore, highScorePosition, Color.Black);
            _retryButton.Draw(spriteBatch);
            _returnButton.Draw(spriteBatch);
            spriteBatch.End();
        }

        /// <summary>
        /// Called when entering the lose mode state, initializes necessary components.
        /// </summary>
        public void Enter()
        {
            _returnButton.OnSceneChanged += OnGoto; // Subscribe to scene change events
            _retryButton.OnSceneChanged += OnGoto;
            _retryButton.SceneName = _gameStateManager.PreviousState; // Set the retry button to return to the previous state
            _returnButton.SceneName = "Menu"; // Set the return button to go to the menu
            _highScoreManager.LoadHighScore(); // Load high score data
        }

        /// <summary>
        /// Called when exiting the lose mode state, performs cleanup if necessary.
        /// </summary>
        public void Exit()
        {
            _retryButton.OnSceneChanged -= OnGoto; // Unsubscribe from scene change events
            _returnButton.OnSceneChanged -= OnGoto;
        }

        /// <summary>
        /// Handles scene changes based on button clicks.
        /// </summary>
        /// <param name="sceneName">The name of the scene to change to.</param>
        private void OnGoto(string sceneName)
        {
            _gameStateManager.ChangeState(sceneName); // Change the game state
        }

        /// <summary>
        /// Updates the lose mode state, including button interactions.
        /// </summary>
        /// <param name="gameTime">The time elapsed since the last update.</param>
        /// <param name="isActive">Indicates whether the game is active.</param>
        public void Update(GameTime gameTime, bool isActive)
        {
            _retryButton.Update(gameTime, isActive); // Update retry button
            _returnButton.Update(gameTime, isActive); // Update return button
        }
    }
}

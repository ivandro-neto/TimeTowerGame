using System;
using Entities.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeTowerGame;

namespace Entities.GameStates
{
    /// <summary>
    /// Represents the game state that is shown when the player wins the game.
    /// Displays the current score, high score, and provides options to retry or return to the menu.
    /// </summary>
    public class WinnerModeState : IGameState
    {
        private GameStateManager _gameStateManager;
        private HighScoreManager _highScoreManager;
        private Button _returnButton;
        private Button _retryButton;
        private ScoreManager _scoreManager;
        private SpriteFont _regular;
        private SpriteFont _header;

        /// <summary>
        /// Initializes a new instance of the <see cref="WinnerModeState"/> class.
        /// </summary>
        /// <param name="regular">The font used for regular text.</param>
        /// <param name="header">The font used for header text.</param>
        /// <param name="scoreManager">Manages the current score of the player.</param>
        /// <param name="highScoreManager">Manages the high score.</param>
        /// <param name="gameStateManager">Manages transitions between different game states.</param>
        /// <param name="buttonTexture">The texture used for the buttons.</param>
        public WinnerModeState(SpriteFont regular, SpriteFont header, ScoreManager scoreManager, HighScoreManager highScoreManager, GameStateManager gameStateManager, Texture2D buttonTexture)
        {
            _regular = regular;
            _header = header;
            _scoreManager = scoreManager;
            _highScoreManager = highScoreManager;
            Rectangle rectangle = new Rectangle(0, 0, 150, 50);
            buttonTexture.SetData(new[] { Color.White }); // Button background color
            _gameStateManager = gameStateManager;
            _retryButton = new Button(buttonTexture, _regular, "Retry", rectangle, new(0, 0), Color.Black, Color.Black, 1);
            _returnButton = new Button(buttonTexture, _regular, "Exit", rectangle, new(0, 0), Color.Black, Color.Black, 1);
        }

        /// <summary>
        /// Draws the winner screen, including the player's score, high score, and buttons for retrying or returning to the menu.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used to draw textures and fonts.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            string score = _scoreManager.CurrentScore.ToString();
            string highScore = _highScoreManager.HighScore.ToString();

            // Get the screen dimensions
            var screenWidth = spriteBatch.GraphicsDevice.Viewport.Width;
            var screenHeight = spriteBatch.GraphicsDevice.Viewport.Height;

            // Calculate positions for centering
            Vector2 titlePosition = new Vector2((screenWidth - _header.MeasureString("Your Win").X) / 2, 30);
            Vector2 scoreLabelPosition = new Vector2((screenWidth - _header.MeasureString("Your Score").X) / 2, 80);
            Vector2 scorePosition = new Vector2((screenWidth - _header.MeasureString(score).X) / 2, 140);
            Vector2 highScoreLabelPosition = new Vector2((screenWidth - _header.MeasureString("High Score").X) / 2, 200);
            Vector2 highScorePosition = new Vector2((screenWidth - _header.MeasureString(highScore).X) / 2, 260);

            // Update button positions dynamically
            _retryButton.Position = new Vector2((screenWidth - _retryButton.Rectangle.Width) / 2, 400);
            _returnButton.Position = new Vector2((screenWidth - _returnButton.Rectangle.Width) / 2, 460);

            spriteBatch.Begin();
            spriteBatch.DrawString(_header, "Your Win", titlePosition, Color.Black);
            spriteBatch.DrawString(_header, "Your Score", scoreLabelPosition, Color.Black);
            spriteBatch.DrawString(_header, score, scorePosition, Color.Black);
            spriteBatch.DrawString(_header, "High Score", highScoreLabelPosition, Color.Black);
            spriteBatch.DrawString(_header, highScore, highScorePosition, Color.Black);
            _retryButton.Draw(spriteBatch);
            _returnButton.Draw(spriteBatch);
            spriteBatch.End();
        }

        /// <summary>
        /// Called when entering the winner mode. Adds event handlers for button actions and loads the high score.
        /// </summary>
        public void Enter()
        {
            _returnButton.OnSceneChanged += OnGoto;
            _retryButton.OnSceneChanged += OnGoto;
            _retryButton.SceneName = _gameStateManager.PreviousState;
            _returnButton.SceneName = "Menu";
            _highScoreManager.LoadHighScore();
        }

        /// <summary>
        /// Called when exiting the winner mode. Removes event handlers for button actions.
        /// </summary>
        public void Exit()
        {
            _retryButton.OnSceneChanged -= OnGoto;
            _returnButton.OnSceneChanged -= OnGoto;
        }

        /// <summary>
        /// Handles updating the state of the winner mode, including button interactions.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="isActive">Indicates whether the game is active or not.</param>
        public void Update(GameTime gameTime, bool isActive)
        {
            _retryButton.Update(gameTime, isActive);
            _returnButton.Update(gameTime, isActive);
        }

        /// <summary>
        /// Handles transitions between scenes based on the button clicked.
        /// </summary>
        /// <param name="sceneName">The name of the scene to transition to.</param>
        private void OnGoto(string sceneName)
        {
            _gameStateManager.ChangeState(sceneName);
        }
    }
}

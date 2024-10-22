using System;
using Entities.Game;
using Entities.GameStates;
using Entities.HUD;
using Entities.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entities.Scenes
{
    /// <summary>
    /// Represents the timekeeper game mode state, managing the game board, scoring, and timers.
    /// </summary>
    public class TimeKeeperModeState : IGameState
    {
        // Game board for the timekeeper mode
        private Board _board;

        // Score manager for tracking the player's score and lives
        private ScoreManager _scoreManager;

        // Timer manager for handling time-related functionality
        private TimerManager _timerManager;

        // Score HUD for displaying the score and lives on the screen
        private ScoreHUD _scoreHUD;

        // Timer HUD for displaying the remaining time on the screen
        private TimerHUD _timerHUD;

        // Constant values for time management
        private const int MILISECS = 1000; // Milliseconds per second
        private const int SECSRESTORE = 10; // Seconds to restore to the timer

        // Counter for scoring events
        private int _scoreCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeKeeperModeState"/> class.
        /// </summary>
        /// <param name="board">The game board for the timekeeper mode.</param>
        /// <param name="spriteFont">The font used for rendering the score and timer HUDs.</param>
        /// <param name="scoreManager">The score manager for tracking scores and lives.</param>
        /// <param name="timerManager">The timer manager for handling game timers.</param>
        public TimeKeeperModeState(Board board, SpriteFont spriteFont, ScoreManager scoreManager, TimerManager timerManager)
        {
            _board = board; // Assign the game board
            _scoreManager = scoreManager; // Assign the score manager
            _timerManager = timerManager; // Assign the timer manager
            _scoreHUD = new ScoreHUD(spriteFont, new Vector2(10, 10), _scoreManager); // Initialize the score HUD
            _timerHUD = new TimerHUD(spriteFont, new Vector2(280, 50), _timerManager); // Initialize the timer HUD
        }

        /// <summary>
        /// Draws the current state to the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for drawing.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            _board.Draw(spriteBatch); // Draw the game board
            _scoreHUD.Draw(spriteBatch, _scoreManager.CurrentScore, _scoreManager.Lifes); // Draw the score HUD
            _timerHUD.Draw(spriteBatch, _timerManager.RemainingTime); // Draw the timer HUD
            spriteBatch.End();
        }

        /// <summary>
        /// Called when entering the timekeeper mode state, initializes the score and timer.
        /// </summary>
        public void Enter()
        {
            // Set default values
            _scoreManager.CurrentScore = 0; // Reset the current score
            _scoreManager.Lifes = 5; // Reset the lives
            _timerManager.RemainingTime = 120 * MILISECS; // Set the remaining time in milliseconds
            _timerManager.IsRunning = true; // Start the timer
            _scoreCounter = 0; // Initialize the score counter
            _scoreManager.ScoreUpdated += OnScoreUpdated; // Subscribe to score updates
        }

        /// <summary>
        /// Called when exiting the timekeeper mode state, performs cleanup if necessary.
        /// </summary>
        public void Exit()
        {
            _scoreManager.ScoreUpdated -= OnScoreUpdated; // Unsubscribe from score updates
        }

        /// <summary>
        /// Updates the timekeeper mode state, including the game board, scoring, and timer management.
        /// </summary>
        /// <param name="gameTime">The time elapsed since the last update.</param>
        /// <param name="isActive">Indicates whether the game is active.</param>
        public void Update(GameTime gameTime, bool isActive)
        {
            _board?.Update(gameTime); // Update the game board
            int pointGained = _board.Score();
            _scoreManager.CurrentScore += pointGained; // Update the score based on the board's score
            _scoreCounter += pointGained;
            // Restore time every 50 points scored
                Console.WriteLine(_scoreCounter);
            if (_scoreCounter >= 50)
            {
                _timerManager.RemainingTime += SECSRESTORE * MILISECS; // Increase remaining time
                Console.WriteLine(_scoreCounter);
                _scoreCounter -= 50; // Reset score counter
            }

            // Check if the player has failed a swap and decrement lives if so
            if (_board.FailSwap())
            {
                _scoreManager.Lifes--; // Decrement lives
            }

            _timerManager.Update(gameTime); // Update the timer manager
        }

        /// <summary>
        /// Handles the score updated event, incrementing the score counter.
        /// </summary>
        /// <param name="score">The current score.</param>
        /// <param name="goal">The score goal.</param>
        protected void OnScoreUpdated(int score, int goal)
        {
        }
    }
}

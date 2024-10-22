using Entities.Game;
using Entities.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entities.Scenes
{
    /// <summary>
    /// Represents the zen game mode state, managing the game board.
    /// </summary>
    public class ZenModeState : IGameState
    {
        // Game board for the zen mode
        private Board _board;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZenModeState"/> class.
        /// </summary>
        /// <param name="board">The game board for the zen mode.</param>
        public ZenModeState(Board board)
        {
            _board = board; // Assign the game board
        }

        /// <summary>
        /// Draws the current state to the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for drawing.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            _board.Draw(spriteBatch); // Draw the game board
            spriteBatch.End();
        }

        /// <summary>
        /// Called when entering the zen mode state, initializes necessary components.
        /// </summary>
        public void Enter()
        {
            // Initialization logic can be added here if necessary
        }

        /// <summary>
        /// Called when exiting the zen mode state, performs cleanup if necessary.
        /// </summary>
        public void Exit()
        {
            // Cleanup logic can be added here if necessary
        }

        /// <summary>
        /// Updates the zen mode state, including the game board.
        /// </summary>
        /// <param name="gameTime">The time elapsed since the last update.</param>
        /// <param name="isActive">Indicates whether the game is active.</param>
        public void Update(GameTime gameTime, bool isActive)
        {
            _board?.Update(gameTime); // Update the game board
        }
    }
}

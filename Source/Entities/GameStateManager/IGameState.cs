using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entities.GameStates
{
    /// <summary>
    /// Interface for defining the basic structure of a game state.
    /// Game states represent different scenes or modes in the game.
    /// </summary>
    public interface IGameState
    {
        /// <summary>
        /// Method that is called when the game state is entered (activated).
        /// </summary>
        void Enter();

        /// <summary>
        /// Method that is called when the game state is exited (deactivated).
        /// </summary>
        void Exit();

        /// <summary>
        /// Updates the game state logic.
        /// </summary>
        /// <param name="gameTime">Snapshot of game timing data.</param>
        /// <param name="isActive">Indicates if the game window is active.</param>
        void Update(GameTime gameTime, bool isActive);

        /// <summary>
        /// Draws the game state's visual elements.
        /// </summary>
        /// <param name="spriteBatch">Used for drawing textures and sprites.</param>
        void Draw(SpriteBatch spriteBatch);
    }
}

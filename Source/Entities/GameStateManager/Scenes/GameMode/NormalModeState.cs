using System;
using Entities.Game;
using Entities.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Represents the normal game mode state, handling the game board, scoring, and user interface elements.
/// </summary>
public class NormalModeState : IGameState
{
    // Game board for the normal mode
    private Board _board;

    // Score manager for tracking the player's score and lives
    private ScoreManager _scoreManager;

    // Score HUD for displaying the score and lives on the screen
    private ScoreHUD _scoreHUD;

    /// <summary>
    /// Initializes a new instance of the <see cref="NormalModeState"/> class.
    /// </summary>
    /// <param name="board">The game board for the normal mode.</param>
    /// <param name="spriteFont">The font used for rendering the score HUD.</param>
    /// <param name="scoreManager">The score manager for tracking scores and lives.</param>
    public NormalModeState(Board board, SpriteFont spriteFont, ScoreManager scoreManager)
    {
        _board = board;  // Assign the game board
        _scoreManager = scoreManager;  // Assign the score manager
        _scoreHUD = new ScoreHUD(spriteFont, new Vector2(10, 10), _scoreManager);  // Initialize the score HUD
    }

    /// <summary>
    /// Draws the current state to the screen.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used for drawing.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        _board.Draw(spriteBatch);  // Draw the game board
        _scoreHUD.Draw(spriteBatch, _scoreManager.CurrentScore, _scoreManager.Lifes);  // Draw the score HUD
        spriteBatch.End();
    }

    /// <summary>
    /// Called when entering the normal mode state, initializes the score and lives.
    /// </summary>
    public void Enter()
    {
        _scoreManager.CurrentScore = 0;  // Reset the current score
        _scoreManager.Lifes = 3;  // Reset the lives
    }

    /// <summary>
    /// Called when exiting the normal mode state, performs cleanup if necessary.
    /// </summary>
    public void Exit()
    {
        // Cleanup logic can be added here if needed
    }

    /// <summary>
    /// Updates the normal mode state, including the game board and score management.
    /// </summary>
    /// <param name="gameTime">The time elapsed since the last update.</param>
    /// <param name="isActive">Indicates whether the game is active.</param>
    public void Update(GameTime gameTime, bool isActive)
    {
        _board?.Update(gameTime);  // Update the game board
        
        _scoreManager.CurrentScore += _board.Score();  // Update the score based on the board's score

        // Check if the player has failed a swap and decrement lives if so
        if (_board.FailSwap())
        {
            _scoreManager.Lifes--;
        }
    }
}

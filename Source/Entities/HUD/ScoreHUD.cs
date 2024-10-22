using System; // Importing the System namespace for general functionalities.
using Microsoft.Xna.Framework; // Importing the Microsoft.Xna.Framework namespace for game framework functionalities.
using Microsoft.Xna.Framework.Graphics; // Importing the Microsoft.Xna.Framework.Graphics namespace for rendering graphics.

public class ScoreHUD
{
    private SpriteFont _font; // Font used for drawing the score and lives on the screen.
    private Vector2 _position; // Position where the HUD will be drawn on the screen.
    private float[] _spacing = {10, 40, 500}; // Array to define spacing for drawing (left, top, right).

    // Constructor for the ScoreHUD class.
    public ScoreHUD(SpriteFont font, Vector2 position, ScoreManager scoreManager)
    {
        _font = font; // Initialize the font.
        _position = position; // Set the position for the HUD.

        // Subscribe to the score update event.
        scoreManager.ScoreUpdated += Update; // Calls the Update method when the score is updated.
    }

    // Method to update the HUD with the current score and lives.
    public void Update(int score, int lifes)
    {
        // Here you can update the HUD if necessary.
        // For example: just printing to the console (currently not implemented).
    }

    // Method to draw the HUD on the screen.
    public void Draw(SpriteBatch spriteBatch, int score, int lifes)
    {
        // Draws the number of lives on the screen.
        spriteBatch.DrawString(_font, $"lifes: {lifes}", 
            new Vector2(_position.X + _spacing[2], _position.Y + _spacing[1]), Color.Black);

        // Draws the current score on the screen.
        spriteBatch.DrawString(_font, $"Score: {score}", 
            new Vector2(_position.X + _spacing[0], _position.Y + _spacing[1]), Color.Black);
    }
}

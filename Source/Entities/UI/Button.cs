using System; // Importing system namespace for general functionalities.
using Microsoft.Xna.Framework; // Importing the MonoGame framework for game development functionalities.
using Microsoft.Xna.Framework.Graphics; // Importing the MonoGame graphics functionalities.
using Microsoft.Xna.Framework.Input; // Importing the MonoGame input functionalities.
using TimeTowerGame.Source.Delegators; // Importing event delegators from your project.

namespace Entities.UI // Namespace for the UI-related classes.
{
    // Class representing a clickable button in the UI.
    public class Button
    {
        private Texture2D _texture; // Texture for the button background.
        private SpriteFont _font; // Font for the button text.
        private Rectangle _buttonBounds; // Area of the button.
        private string _text; // Text displayed on the button.
        private Color _textColor; // Color of the button text.
        private Color _borderColor; // Color of the button border.
        private int _borderThickness; // Thickness of the button border.
        private Vector2 _position; // Position of the button.
        private float _delay = 500f; // Delay for click response.

        // Property for the button's position.
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                // Update the button bounds whenever the position is set.
                _buttonBounds.X = (int)_position.X;
                _buttonBounds.Y = (int)_position.Y;
            }
        }

        // Property for the button's rectangle area.
        public Rectangle Rectangle
        {
            get { return _buttonBounds; }
            set
            {
                _buttonBounds = value;
                // Also update the position if the rectangle is set directly.
                _position = new Vector2(_buttonBounds.X, _buttonBounds.Y);
            }
        }

        private MouseState _previousMouseState; // Previous state of the mouse.
        public bool IsClicked { get; private set; } // Flag to check if the button is clicked.

        private string _sceneName; // Name of the scene linked to the button.

        // Property for the scene name.
        public string SceneName
        {
            get { return _sceneName; }
            set
            {
                _sceneName = value;
                Console.WriteLine($"Linked to::{_sceneName}"); // Log the scene linkage.
            }
        }

        public event GoToEventHandler OnSceneChanged; // Event triggered when the scene changes.

        // Constructor for initializing the button.
        public Button(Texture2D texture, SpriteFont font, string text, Rectangle buttonBounds, Vector2 position, Color textColor, Color borderColor, int borderThickness)
        {
            _texture = texture; // Assigns the button texture.
            _font = font; // Assigns the button font.
            _text = text; // Assigns the button text.
            _buttonBounds = buttonBounds; // Assigns the button bounds.
            _textColor = textColor; // Assigns the text color.
            _borderColor = borderColor; // Assigns the border color.
            _borderThickness = borderThickness; // Assigns the border thickness.
            _previousMouseState = Mouse.GetState(); // Capture the initial mouse state.
            Position = position; // Use the property to set the initial position and update bounds.
        }

        // Update method for handling button state.
        public void Update(GameTime gameTime, bool isActive)
        {
            if (!isActive)
            {
                // If the game window is not active, ignore button updates.
                return;
            }

            MouseState currentMouseState = Mouse.GetState(); // Get the current state of the mouse.
            _delay -= (float)gameTime.ElapsedGameTime.TotalMilliseconds; // Reduce delay based on elapsed game time.

            if (_delay < 0)
            {
                // Check if the mouse is within the button bounds and if it has been clicked.
                if (_buttonBounds.Contains(currentMouseState.Position))
                {
                    if (currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                    {
                        IsClicked = true; // Set clicked flag to true.
                        OnSceneChanged?.Invoke(_sceneName); // Trigger the scene change event.
                    }
                }
            }

            // Update the previous mouse state.
            _previousMouseState = currentMouseState;
        }

        // Draw method to render the button on the screen.
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the button background (texture is optional).
            if (_texture != null)
            {
                spriteBatch.Draw(_texture, _buttonBounds, Color.White);
            }

            // Draw the button border.
            DrawBorder(spriteBatch, _buttonBounds, _borderColor, _borderThickness);

            // Center the text within the button.
            Vector2 textSize = _font.MeasureString(_text); // Measure the size of the text.
            Vector2 textPosition = new Vector2(
                _buttonBounds.X + (_buttonBounds.Width - textSize.X) / 2,
                _buttonBounds.Y + (_buttonBounds.Height - textSize.Y) / 2
            );

            // Draw the text.
            spriteBatch.DrawString(_font, _text, textPosition, _textColor);
        }

        // Method to draw the button border.
        private void DrawBorder(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int thickness)
        {
            // Draw the borders of the rectangle (top, bottom, left, right).
            Texture2D borderTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1); // Create a 1x1 texture for borders.
            borderTexture.SetData(new[] { Color.White }); // Set texture color to white.

            // Draw the top border.
            spriteBatch.Draw(borderTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
            // Draw the bottom border.
            spriteBatch.Draw(borderTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color);
            // Draw the left border.
            spriteBatch.Draw(borderTexture, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
            // Draw the right border.
            spriteBatch.Draw(borderTexture, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color);
        }
    }
}

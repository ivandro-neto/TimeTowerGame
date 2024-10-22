using Log = System.Console; // Alias for the Console class to log messages.
using Microsoft.Xna.Framework; // Importing the MonoGame framework for game development functionalities.
using Microsoft.Xna.Framework.Graphics; // Importing the MonoGame graphics functionalities.

namespace Entities.Pieces // Namespace for the game piece-related classes.
{
    public abstract class Piece
    {
        private Texture2D _texture; // Texture for the piece, representing its visual appearance.
        private Vector2 _position; // Position of the piece in the game world.
        private int _type; // Type identifier for the piece.
        private bool _isMatched; // Indicates whether the piece is matched with another piece.

        private float _opacity = 1.0f; // Initial opacity of the piece (1.0 = fully visible).
        private float _fadeSpeed = 0.0001f; // Speed of the fade effect.
        private bool _isFadingOut = false; // Control flag for fade-out state.
        private bool _isFadingIn = false; // Control flag for fade-in state.

        private readonly int _width = 32; // Width of the piece used for rendering.

        /* Getters */
        public bool IsMatched
        {
            get { return _isMatched; } // Returns whether the piece is matched.
            set { _isMatched = value; } // Sets the matched state of the piece.
        }
        
        public Vector2 Position
        {
            get { return _position; } // Returns the current position of the piece.
            set { _position = value; } // Sets the position of the piece.
        }
        
        public Texture2D Texture
        {
            get { return _texture; } // Returns the texture of the piece.
            set { _texture = value; } // Sets the texture of the piece.
        }
        
        public int Type
        {
            get { return _type; } // Returns the type of the piece.
        }

        /* Methods */
        // Constructor to initialize the piece with a texture, position, and type.
        public Piece(Texture2D texture, Vector2 position, int type)
        {
            _texture = texture; // Assigns the texture.
            _position = position; // Assigns the position.
            _type = type; // Assigns the type.
            _isMatched = false; // Initializes the matched state as false.
        }

        // Method to set the match state of the piece.
        public void SetMatchState(bool state)
        {
            _isMatched = state; // Sets the matched state.
        }

        // Method to draw the piece on the screen.
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            float aspectRatio = (float)_texture.Height / _texture.Width; // Calculates the aspect ratio of the texture.
            int height = (int)(_width * aspectRatio); // Calculates the height based on the width and aspect ratio.
            Color color = new Color(1f, 1f, 1f, _opacity); // Creates a color with the current opacity.
            Rectangle rectangle = new Rectangle((int)_position.X, (int)_position.Y, _width, height); // Defines the rectangle for rendering.
            spriteBatch.Draw(_texture, rectangle, color); // Draws the texture with the defined rectangle and color.
        }

        // Method to update the state of the piece each frame.
        public virtual void Update(GameTime gameTime)
        {
            // Updates the opacity for fade out
            if (_isFadingOut)
            {
                _opacity -= _fadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; // Decreases opacity over time.
                if (_opacity <= 0)
                {
                    _opacity = 0; // Ensures opacity doesn't go below 0.
                    _isFadingOut = false; // Stops the fade-out effect.
                }
            }

            // Updates the opacity for fade in
            if (_isFadingIn)
            {
                _opacity += _fadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; // Increases opacity over time.
                if (_opacity >= 1)
                {
                    _opacity = 1; // Ensures opacity doesn't exceed 1.
                    _isFadingIn = false; // Stops the fade-in effect.
                }
            }
        }

        // Animation methods
        // Starts the fade-out animation.
        public void StartFadeOut()
        {
            _isFadingOut = true; // Activates the fade-out state.
            _isFadingIn = false; // Ensures that only one fade effect is active.
        }

        // Starts the fade-in animation.
        public void StartFadeIn()
        {
            _isFadingIn = true; // Activates the fade-in state.
            _isFadingOut = false; // Ensures that only one fade effect is active.
        }

        // Checks if a point is within the bounds of the piece.
        public bool ContainsPoint(Vector2 point)
        {
            float aspectRatio = (float)_texture.Height / _texture.Width; // Calculates the aspect ratio.
            int height = (int)(_width * aspectRatio); // Calculates the height based on the width and aspect ratio.
            Rectangle rectangle = new Rectangle((int)_position.X, (int)_position.Y, _width, height); // Defines the rectangle for the piece.

            return rectangle.Contains(point); // Returns true if the point is within the rectangle.
        }
    }

    // Derived classes representing specific pieces.
    public class Xolotl : Piece
    {
        private const int type = 1; // Type identifier for Xolotl.
        public Xolotl(Texture2D texture, Vector2 position) : base(texture, position, type) { } // Constructor initializing Xolotl.
    }

    public class Nuwa : Piece
    {
        private const int type = 2; // Type identifier for Nuwa.
        public Nuwa(Texture2D texture, Vector2 position) : base(texture, position, type) { } // Constructor initializing Nuwa.
    }

    public class Fuxi : Piece
    {
        private const int type = 3; // Type identifier for Fuxi.
        public Fuxi(Texture2D texture, Vector2 position) : base(texture, position, type) { } // Constructor initializing Fuxi.
    }

    public class Horus : Piece
    {
        private const int type = 4; // Type identifier for Horus.
        public Horus(Texture2D texture, Vector2 position) : base(texture, position, type) { } // Constructor initializing Horus.
    }
}

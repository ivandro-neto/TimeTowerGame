using System; // Importing system namespace for general functionalities.
using Microsoft.Xna.Framework; // Importing the MonoGame framework for game development functionalities.
using Microsoft.Xna.Framework.Graphics; // Importing the MonoGame graphics functionalities.

namespace Entities.Pieces // Namespace for the piece-related classes.
{
    // Interface for piece factories, defining a method for creating pieces.
    public interface IPieceFactory
    {
        // Method to create a piece at a specified position.
        public Piece CreatePiece(Vector2 position);
    }

    // Concrete implementation of the IPieceFactory interface.
    public class BasicPieceFactory : IPieceFactory
    {
        private readonly Texture2D _xolotlTexture; // Texture for the Xolotl piece.
        private readonly Texture2D _nuwaTexture; // Texture for the Nuwa piece.
        private readonly Texture2D _fuxiTexture; // Texture for the Fuxi piece.
        private readonly Texture2D _horusTexture; // Texture for the Horus piece.
        private static readonly Random random = new(); // Static instance of Random for generating random numbers.

        // Factory constructor which receives all pieces' textures.
        public BasicPieceFactory(Texture2D xolotlTexture, Texture2D nuwaTexture, Texture2D fuxiTexture, Texture2D horusTexture)
        {
            _xolotlTexture = xolotlTexture; // Assigns the Xolotl texture.
            _nuwaTexture = nuwaTexture; // Assigns the Nuwa texture.
            _fuxiTexture = fuxiTexture; // Assigns the Fuxi texture.
            _horusTexture = horusTexture; // Assigns the Horus texture.
        }

        // Generates a random type for the piece (1-4).
        public static int RandomType() => random.Next(1, 5);

        // Creates a new Piece based on a random type.
        public Piece CreatePiece(Vector2 position) 
        {
            int type = RandomType(); // Get a random piece type.
            return type switch // Switch statement to determine the type of piece to create.
            {
                1 => new Xolotl(_xolotlTexture, position), // Creates Xolotl piece.
                2 => new Nuwa(_nuwaTexture, position), // Creates Nuwa piece.
                3 => new Fuxi(_fuxiTexture, position), // Creates Fuxi piece.
                4 => new Horus(_horusTexture, position), // Creates Horus piece.
                _ => throw new InvalidOperationException("Wrong Piece Type.") // Throws exception for invalid type.
            };
        }   
    }
}

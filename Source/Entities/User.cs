namespace Entities // Namespace for entity-related classes.
{
    // Class representing a user in the game.
    public class User
    {
        // Unique identifier for the user.
        public int Id { get; private set; } 
        
        // Name of the user.
        public string Name { get; private set; } 
        
        // User's highest score in the game.
        public int HighScore { get; set; } 
        
        // Default constructor.
        public User()
        {
            // Initializes a new instance of the User class without parameters.
        }

        // Constructor for initializing a user with specified parameters.
        public User(int id, string name, int highScore)
        {
            Id = id; // Set the user's unique identifier.
            Name = name; // Set the user's name.
            HighScore = highScore; // Set the user's high score.
        }
    }
}

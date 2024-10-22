using System; // Importing the System namespace for general functionalities.
using System.IO; // Importing the System.IO namespace for file operations.
using System.Text.Json; // Importing the System.Text.Json namespace for JSON serialization and deserialization.

namespace TimeTowerGame
{
    public class HighScoreManager
    {
        private readonly string _directoryPath; // Path to the directory for storing high score data.
        private readonly string _filePath; // Path to the high score JSON file.

        public int HighScore { get; private set; } // Property to store the high score.

        // Constructor for the HighScoreManager class.
        public HighScoreManager(string appName)
        {
            // Define the path of the folder in AppData\Roaming.
            _directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName);

            // Define the full path of the highScore.json file.
            _filePath = Path.Combine(_directoryPath, "highScore.json");

            // Ensure that the AppData directory exists; create it if it doesn't.
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }

            // Load the high score from the file.
            LoadHighScore();
        }

        // Loads the high score from the JSON file.
        public void LoadHighScore()
        {
            if (File.Exists(_filePath)) // Check if the high score file exists.
            {
                try
                {
                    string json = File.ReadAllText(_filePath); // Read the JSON data from the file.
                    HighScore = JsonSerializer.Deserialize<int>(json); // Deserialize the JSON data into an integer.
                }
                catch (Exception ex)
                {
                    // Handle any errors during loading, and set the high score to 0.
                    Console.WriteLine($"Error loading high score: {ex.Message}");
                    HighScore = 0; // Initialize to 0 if there is an error.
                }
            }
            else
            {
                HighScore = 0; // If the file is not found, initialize the high score to 0.
            }
        }

        // Saves the high score to the JSON file if the new score is greater than the current high score.
        public void SaveHighScore(int newScore)
        {
            if (newScore > HighScore) // Check if the new score exceeds the current high score.
            {
                HighScore = newScore; // Update the high score.

                try
                {
                    string json = JsonSerializer.Serialize(HighScore); // Serialize the high score to JSON.
                    File.WriteAllText(_filePath, json); // Write the JSON data to the file.
                }
                catch (Exception ex)
                {
                    // Handle any errors during saving.
                    Console.WriteLine($"Error saving high score: {ex.Message}");
                }
            }
        }
    }
}

using System;
using System.IO;

namespace Utilities.Tokens
{
    // This class is responsible for storing and retrieving the authentication token.
    public class TokenStorage
    {
        // File path for storing the token in the application data directory.
        private string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "game-token.txt");

        // Saves the token to a file.
        public void SaveToken(string token)
        {
            File.WriteAllText(_filePath, token); // Writes the token to the specified file.
        }

        // Loads the token from the file, returning null if it does not exist.
        public string LoadToken()
        {
            if (File.Exists(_filePath)) // Check if the file exists.
            {
                return File.ReadAllText(_filePath); // Read and return the token.
            }
            return null; // Return null if the file does not exist.
        }
    }
}

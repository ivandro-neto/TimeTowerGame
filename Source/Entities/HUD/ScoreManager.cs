using TimeTowerGame.Source.Delegators; // Importing the Delegators namespace for event handling.

public class ScoreManager
{
    private int _currentScore; // Variable to hold the current score of the player.
    private int _lifes; // Variable to hold the number of lives the player has.

    // Event that is triggered when the score is updated.
    public event ScoreUpdatedEventHandler ScoreUpdated; 

    // Property to get and set the current score.
    public int CurrentScore
    {
        get => _currentScore; // Gets the current score.
        set
        {
            _currentScore = value; // Sets the current score.
            OnScoreUpdated(); // Triggers the event when the score changes.
        }
    }

    // Property to get and set the number of lives.
    public int Lifes
    {
        get => _lifes; // Gets the number of lives.
        set
        {
            _lifes = value; // Sets the number of lives.
            OnScoreUpdated(); // Triggers the event when the lives change.
        }
    }

    // Protected method to notify subscribers when the score is updated.
    protected virtual void OnScoreUpdated()
    {
        ScoreUpdated?.Invoke(_currentScore, _lifes); // Notifies all subscribers of the score update event.
    }
}

namespace TimeTowerGame.Source.Delegators
{
    /// <summary>
    /// Delegate for handling the score update event. Triggered when the score changes.
    /// </summary>
    /// <param name="score">The current score.</param>
    /// <param name="goal">The target goal score.</param>
    public delegate void ScoreUpdatedEventHandler(int score, int goal);

    /// <summary>
    /// Delegate for handling the timer update event. Triggered when the timer updates.
    /// </summary>
    /// <param name="time">The current time on the timer.</param>
    /// <param name="state">The state of the timer (running or stopped).</param>
    public delegate void TimerUpdatedEventHandler(float time, bool state);

    /// <summary>
    /// Delegate for handling scene transitions. Triggered when changing scenes.
    /// </summary>
    /// <param name="sceneName">The name of the scene to transition to.</param>
    public delegate void GoToEventHandler(string sceneName);

    /// <summary>
    /// Delegate for handling the event when the game exits.
    /// </summary>
    public delegate void ExitGameEventHandler();

    /// <summary>
    /// EventArgs class used to carry information related to the winning event.
    /// </summary>
    public class WinningEventArgs
    {
        /// <summary>
        /// Indicates whether the player has won.
        /// </summary>
        public bool IsWinner { get; }

        /// <summary>
        /// Indicates whether the game is currently being played.
        /// </summary>
        public bool IsPlaying { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WinningEventArgs"/> class.
        /// </summary>
        /// <param name="isWinner">True if the player has won; otherwise, false.</param>
        /// <param name="playing">True if the game is still in progress; otherwise, false.</param>
        public WinningEventArgs(bool isWinner, bool playing)
        {
            IsWinner = isWinner;
            IsPlaying = playing;
        }
    }

    /// <summary>
    /// Delegate for handling the winning event. Triggered when the player wins or loses.
    /// </summary>
    /// <param name="sender">The object that triggers the event.</param>
    /// <param name="e">The event arguments containing winning information.</param>
    public delegate void WinningEventHandler(object sender, WinningEventArgs e);
}

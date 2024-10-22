using Microsoft.Xna.Framework; // Importing the MonoGame framework for game development functionalities.
using TimeTowerGame.Source.Delegators; // Importing the Delegators namespace for event handling.

namespace Entities.Timers // Namespace for timer-related classes.
{
    public class TimerManager
    {
        private float _totalTime; // Variable to hold the total time for the timer in milliseconds.
        private bool _isRunning; // Variable to track the running state of the timer.

        // Event to notify subscribers when the timer is updated.
        public event TimerUpdatedEventHandler TimerUpdated;

        // Property to get or set the remaining time of the timer.
        public float RemainingTime 
        {
            get => _totalTime; // Returns the total time remaining.
            set 
            {
                _totalTime = value; // Sets the total time to the specified value.
                OnTimerUpdated(); // Triggers the TimerUpdated event.
            } 
        } 

        // Property to get or set the running state of the timer.
        public bool IsRunning 
        {
            get => _isRunning; // Returns whether the timer is currently running.
            set
            {
                _isRunning = value; // Sets the running state to the specified value.
                OnTimerUpdated(); // Triggers the TimerUpdated event.
            }
        }

        // Method to start the timer.
        public void Start()
        {
            _isRunning = true; // Sets the running state to true.
        }

        // Method to stop the timer.
        public void Stop()
        {
            _isRunning = false; // Sets the running state to false.
        }

        // Method to reset the timer with a new total time.
        public void Reset(float time)
        {
            _totalTime = time; // Sets the total time to the specified value.
        }

        // Method to update the timer based on the elapsed game time.
        public void Update(GameTime gameTime)
        {
            _totalTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds; // Decrease the total time by the elapsed game time in milliseconds.
            if (_totalTime < 0) // Checks if the timer has run out.
            {
                _totalTime = 0; // Sets the total time to zero if it goes negative.
                _isRunning = false; // Stops the timer.
            }
        }

        // Protected method to invoke the TimerUpdated event.
        protected void OnTimerUpdated()
        {
            TimerUpdated?.Invoke(_totalTime, _isRunning); // Notifies all subscribers of the timer update.
        }
    }
}

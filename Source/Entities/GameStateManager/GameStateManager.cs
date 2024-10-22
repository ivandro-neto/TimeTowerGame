using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Entities.Timers;
using TimeTowerGame.Source.Delegators;

namespace Entities.GameStates
{
    /// <summary>
    /// Manages the different game states (scenes) and handles transitions between them.
    /// It also checks and handles winning conditions based on the game state.
    /// </summary>
    public class GameStateManager
    {
        // Current active game state
        private IGameState _currentState;

        // Previously active game state
        private string _previousState;

        // Collection of game states, indexed by their names
        private readonly Dictionary<string, IGameState> _states;

        // Managers for handling score and timer-related functionalities
        private ScoreManager _scoreManager;
        private TimerManager _timerManager;

        /// <summary>
        /// Gets the previous game state name before the current state.
        /// </summary>
        public string PreviousState => _previousState;

        /// <summary>
        /// Event triggered when a winning or losing condition is met.
        /// </summary>
        public event WinningEventHandler IsWinner;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameStateManager"/> class.
        /// </summary>
        /// <param name="scoreManager">Handles score-related logic.</param>
        /// <param name="timerManager">Handles timer-related logic.</param>
        public GameStateManager(ScoreManager scoreManager, TimerManager timerManager)
        {
            _states = new Dictionary<string, IGameState>();
            _scoreManager = scoreManager;
            _timerManager = timerManager;
        }

        /// <summary>
        /// Adds a new game state to the manager.
        /// </summary>
        /// <param name="name">The unique name of the game state.</param>
        /// <param name="gameState">The instance of the game state to be added.</param>
        public void AddState(string name, IGameState gameState)
        {
            _states[name] = gameState;

            // Subscribe to the winning event
            IsWinner += OnPlayerWin;
        }

        /// <summary>
        /// Changes the current game state to the specified new state.
        /// </summary>
        /// <param name="newState">The name of the new game state to transition to.</param>
        public void ChangeState(string newState)
        {
            if (_currentState != null)
                _previousState = GetStateName(_currentState); // Store the current state before changing
            _currentState?.Exit(); // Exit the current state
            _currentState = _states[newState]; // Set the new state
            
            _currentState.Enter(); // Enter the new state
            
            // Debugging information (uncomment for debugging)
            // Console.WriteLine($"previous::{_previousState} current::{current}");
        }

        /// <summary>
        /// Gets the name of a specific game state.
        /// </summary>
        /// <param name="gameState">The game state instance.</param>
        /// <returns>The name of the game state.</returns>
        private string GetStateName(IGameState gameState)
        {
            foreach (var state in _states)
            {
                if (state.Value == gameState)
                    return state.Key;
            }
            return null;
        }

        /// <summary>
        /// Updates the current game state and checks if a winning condition is met.
        /// </summary>
        /// <param name="gameTime">Snapshot of game timing data.</param>
        /// <param name="isActive">Indicates if the game window is active.</param>
        public void Update(GameTime gameTime, bool isActive)
        {
            _currentState?.Update(gameTime, isActive);
            CheckGameWinningState();
        }

        /// <summary>
        /// Checks for winning or losing conditions based on the current state.
        /// </summary>
        private void CheckGameWinningState()
        {
            if (_currentState != null && _currentState == _states["NormalMode"])
                NormalModeWinningCondition();
            else if (_currentState != null && _currentState == _states["TimeKeeperMode"])
                TimerKeeperModeWinningCondition();
        }

        /// <summary>
        /// Draws the current game state's visual elements.
        /// </summary>
        /// <param name="spriteBatch">Used for drawing textures and sprites.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            _currentState?.Draw(spriteBatch);
        }

        /// <summary>
        /// Handles the winning conditions for the "NormalMode" game state.
        /// </summary>
        private void NormalModeWinningCondition()
        {
            if (_scoreManager.Lifes <= 0)
            {
                // Triggers the winning event with losing condition
                IsWinner?.Invoke(this, new WinningEventArgs(false, false));
            }
        }

        /// <summary>
        /// Handles the winning conditions for the "TimeKeeperMode" game state.
        /// </summary>
        private void TimerKeeperModeWinningCondition()
        {
            if (_timerManager.RemainingTime <= 0 && _scoreManager.Lifes > 0)
            {
                // Triggers the winning event with a win condition
                IsWinner?.Invoke(this, new WinningEventArgs(true, false));
            }
            else if (_timerManager.RemainingTime <= 0 || _scoreManager.Lifes <= 0)
            {
                // Triggers the winning event with a losing condition
                IsWinner?.Invoke(this, new WinningEventArgs(false, false));
            }
        }

        /// <summary>
        /// Handles the player winning or losing event when triggered.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The winning event arguments.</param>
        private void OnPlayerWin(object sender, WinningEventArgs e)
        {
            // Debugging information (uncomment for debugging)
            if (e.IsWinner && e.IsPlaying == false)
            {
                // Console.WriteLine("You Win");
            }
            else if (e.IsWinner == false && e.IsPlaying == false)
            {
                // Console.WriteLine("You Lose");
            }
        }
    }
}

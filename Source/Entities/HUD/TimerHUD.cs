using System; // Importing the System namespace for general functionalities.
using Entities.Timers; // Importing the Timers namespace for the TimerManager class.
using Microsoft.Xna.Framework; // Importing the MonoGame framework for game development functionalities.
using Microsoft.Xna.Framework.Graphics; // Importing for graphics rendering functionalities.

namespace Entities.HUD // Namespace for the HUD (Heads-Up Display) components.
{
    public class TimerHUD
    {
        private SpriteFont _font; // Variable to hold the font used for rendering the timer.
        private Vector2 _position; // Variable to hold the position of the timer on the screen.

        // Constructor to initialize the TimerHUD with a font, position, and TimerManager.
        public TimerHUD(SpriteFont font, Vector2 position, TimerManager timerManager)
        {
            _font = font; // Assigns the provided font to the _font variable.
            _position = position; // Assigns the provided position to the _position variable.
            timerManager.TimerUpdated += Update; // Subscribes to the TimerUpdated event from the TimerManager.
        }

        // Method to update the HUD with the current time and state of the timer.
        public void Update(float time, bool state)
        {
            // Uncomment this line just for debugging.
            // Console.WriteLine("TIMER::{0} STATE::{1}", time, state); // Debugging output for timer state and time.
        }

        // Method to draw the timer on the screen using the SpriteBatch.
        public void Draw(SpriteBatch spriteBatch, float time)
        {
            // Convert the elapsed time in milliseconds to minutes and seconds.
            int totalSeconds = (int)time / 1000; // Calculate total seconds.
            int minute = totalSeconds / 60; // Calculate minutes.
            int secs = totalSeconds % 60; // Calculate remaining seconds.

            // Draw the formatted timer string on the screen.
            spriteBatch.DrawString(_font, $"{minute} : {secs}", _position, Color.Black);   
        }
    }
}

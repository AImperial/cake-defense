#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using System.IO;
#endregion using
/// by Torey J. Scheer
/// Made to do easy timing, be it just waiting to do something, or getting a percent of time passed
/// 
/// ???
namespace CakeDefense
{
    class Timer
    {
        #region Attributes
        // tte should not be called, but rather the property, so that it can effect Timer speed.
        TimeSpan startTime, currentTime, tte;
        double speed;
        #endregion Attributes

        #region Constructors
        /// <summary> Default Constructor. </summary>
        public Timer()
        {
            speed = 1;
        }

        /// <summary> Creates Timer with pre-defined speed. </summary>
        public Timer(double speed)
        {
            this.speed = speed;
        }

        /// <summary> Basic Opperating Timer. </summary>
        public Timer(TimeSpan start, TimeSpan timeTillEnd)
            : this()
        {
            startTime = start;
            tte = timeTillEnd;
        }

        /// <summary> Standard Opperating Timer. </summary>
        public Timer(TimeSpan start, TimeSpan timeTillEnd, TimeSpan current)
            : this(start, timeTillEnd)
        {
            currentTime = current;
        }

        /// <summary> Sets every value of a Timer to something. </summary>
        public Timer(TimeSpan start, TimeSpan timeTillEnd, TimeSpan current, double speed)
            : this(start, timeTillEnd, current)
        {
            this.speed = speed;
        }
        #endregion Constructors

        #region Properties

        public float Percent
        {
            get { return (float)(TimeElapsed.TotalMilliseconds / TimeTillEnd.TotalMilliseconds); }
        }

        public double Speed
        {
            get { return speed; }

            set { speed = value; }
        }

        public bool Finished
        {
            get { if (currentTime >= EndTime) { return true; } else { return false; } }
        }

        public TimeSpan StartTime
        {
            get { return startTime; }

            set { startTime = value; }
        }

        /// <summary> Gets the current time the Timer has stored. (To set use Update()) </summary>
        public TimeSpan CurrentTime
        {
            get { return currentTime; }
        }

        /// <summary> The current time until time finishes, in relation to timer speed (including setting value). </summary>
        public TimeSpan TimeTillEnd
        {
            get { return TimeSpan.FromTicks((long)(tte.Ticks / Speed)); }

            set { tte = TimeSpan.FromTicks((long)(value.Ticks * Speed)); }
        }

        public TimeSpan EndTime
        {
            get { return startTime + TimeTillEnd; }
        }

        public TimeSpan TimeElapsed
        {
            get { return currentTime - startTime; }
        }

        #endregion Properties

        #region Methods

        #region Regular

        public void Start(GameTime gameTime, TimeSpan length)
        {
            startTime = gameTime.TotalGameTime;
            tte = length;
        }

        public void Start(GameTime gameTime, int lengthInMilSec)
        {
            startTime = gameTime.TotalGameTime;
            tte = new TimeSpan(0, 0, 0, 0, lengthInMilSec);
        }

        public void Update(GameTime gameTime, int gameSpeed)
        {
            currentTime = gameTime.TotalGameTime;
        }

        public void End()
        {
            startTime = currentTime = tte = TimeSpan.Zero;
            speed = 1;
        }

        public override string ToString()
        {
            return "{Percent Finished: " + Percent +
                "\nStart Time: " + StartTime.ToString("ss") +
                "\nCurrent Time: " + CurrentTime.ToString("ss") +
                "\nEnd Time" + EndTime.ToString("ss") + "}";
        }

        #endregion Regular

        #region Static

        /// <summary> Returns the value as if start is 0 and finish is 1 (if value == start returns 0). </summary>
        /// <param name="start">The percent that should be considered start (0).</param>
        /// <param name="current">The percent to change to a relative percent.</param>
        /// <param name="finish">The percent that should be considered finish (1)</param>
        /// <returns>A relative percent.</returns>
        public static float GetPercentRelative(float start, float current, float finish)
        {
            // (current - start) / TimeTillEnd
            return (current - start) / (finish - start);
        }

        #endregion Static

        #endregion Methods
    }
}

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

namespace CakeDefense
{
    // Constant/Static variables go here
    class Var
    {
        #region Screen/Map Stuff

        public static int TOTAL_WIDTH = 1280;
        public static int TOTAL_HEIGHT = 720;
        public static Rectangle SCREEN_SIZE = new Rectangle(0, 0, TOTAL_WIDTH, TOTAL_HEIGHT);

        public static int TILE_SIZE = 40;

        #endregion Screen/Map Stuff

        #region Tower

        public static int MAX_TOWER_HEALTH = 10;

        #endregion Tower

        #region Enemy

        public static int MAX_ENEMY_HEALTH = 10;
        public static TimeSpan SPAWN_TIME = new TimeSpan(0, 0, 5);
        public const int SPAWN_SPINS = 3;

        #endregion Enemy

        #region Time Stuff

        public static int GAME_SPEED = 1;

        private static double timeDif;
        public static float TimePercentTillComplete(TimeSpan startTime, TimeSpan plusTime, GameTime gameTime)
        {
            timeDif = gameTime.TotalGameTime.TotalMilliseconds - startTime.TotalMilliseconds;

            // returns a number 0-1 if GameTime in not over endtime / under start time.
            return (float)(timeDif / plusTime.TotalMilliseconds);
        }
        #endregion Time Stuff

        #region Colors

        //--

        private static Color testColor;
        public static Color EffectTransparency(float percent, Color clr)
        {
            return Color.FromNonPremultiplied(clr.R, clr.G, clr.B, (byte)(clr.A / percent));
        }
        #endregion Colors
    }
}

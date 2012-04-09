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
        public static Rectangle GAME_AREA = new Rectangle(0, 0, TOTAL_WIDTH, TOTAL_HEIGHT);

        public static int TILE_SIZE = 40;

        #endregion Screen/Map Stuff

        #region Tower

        public static int MAX_TOWER_HEALTH = 10;

        #endregion Tower

        #region Bullet
        public static int BULLET_SPEED = 2;
        #endregion Bullet

        #region Enemy

        public static int MAX_ENEMY_HEALTH = 10;
        public static TimeSpan SPAWN_TIME = new TimeSpan(0, 0, 0, 0, 500);
        public static TimeSpan DESPAWN_TIME = new TimeSpan(0, 0, 1);
        public const int SPAWN_SPINS = 1;
        public static TimeSpan TIME_BETWEEN_SPAWNS = new TimeSpan(0, 0, 1);
        public static TimeSpan TIME_BETWEEN_WAVES = new TimeSpan(0, 0, 10);
        public const int ENEMY_SIZE = 25;

        #endregion Enemy

        #region Time Stuff

        public static int GAME_SPEED = 1;
        public const int FRAME_SPEED = 500;

        private static double timeDif;
        public static float TimePercentTillComplete(TimeSpan startTime, TimeSpan plusTime, GameTime gameTime)
        {
            timeDif = gameTime.TotalGameTime.TotalMilliseconds - startTime.TotalMilliseconds;

            // returns a number 0-1 if GameTime in not over endtime / under start time.
            return (float)(timeDif / plusTime.TotalMilliseconds);
        }
        #endregion Time Stuff

        #region Enums
        public enum EnemyType { Spider }
        public enum TowerType { Basic }
        #endregion Enums

        #region Colors

        public static Color PLACING_TOWER_COLOR = Color.FromNonPremultiplied(255, 255, 255, 150);

        public static Color EffectTransparency(float percent, Color clr)
        {
            return Color.FromNonPremultiplied(clr.R, clr.G, clr.B, (byte)(clr.A * percent));
        }
        #endregion Colors
    }
}

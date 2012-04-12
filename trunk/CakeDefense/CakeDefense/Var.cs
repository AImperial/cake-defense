﻿#region Using Statements
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
        public static int TOTAL_WIDTH = 1280, TOTAL_HEIGHT = 720, TILE_SIZE = 40;
        public static Rectangle SCREEN_SIZE = new Rectangle(0, 0, TOTAL_WIDTH, TOTAL_HEIGHT);
        public static Rectangle GAME_AREA = new Rectangle(0, 0, TOTAL_WIDTH, TOTAL_HEIGHT);
        public static Texture2D BLANK_TEX;
        #endregion Screen/Map Stuff

        #region Cake / HUD
        public static int MAX_CAKE_HEALTH = 5, START_MONEY = 500;
        #endregion Cake / HUD

        #region Tower
        public static int MAX_TOWER_HEALTH = 10;
        #endregion Tower

        #region Trap
        public const int TRAP_SIZE = 30;
        public static TimeSpan TRAP_SHOW_HEALTH_TIME = new TimeSpan(0, 0, 1);
        #endregion Trap

        #region Bullet
        public static float BASE_BULLET_SPEED = 4;
        public const int BULLET_SIZE = 10;
        #endregion Bullet

        #region Enemy
        public static int MAX_ENEMY_HEALTH = 4, SPAWN_SPINS = 1, DEATH_SPINS = 3, ENEMY_SIZE = 25;
        public const float ENEMY_SLOW_CAP = .25f, WAVES_DIFFICULTY_INCREASE = .1f;
        public static TimeSpan SPAWN_TIME = new TimeSpan(0, 0, 0, 0, 500);
        public static TimeSpan DESPAWN_TIME = new TimeSpan(0, 0, 1);
        public static TimeSpan DYING_TIME = new TimeSpan(0, 0, 1);

        public static TimeSpan ENEMY_SHOW_HEALTH_TIME = new TimeSpan(0, 0, 5);
        public static Point HEALTHBAR_SIZE_MAX = new Point(75, 5);
        #endregion Enemy

        #region Time Stuff
        public static int GAME_SPEED = 1;
        public static int FRAME_SPEED = 500;

        public static TimeSpan TIME_BETWEEN_SPAWNS = new TimeSpan(0, 0, 0, 0, 1000);
        public static TimeSpan TIME_BETWEEN_WAVES = new TimeSpan(0, 0, 10);

        public static TimeSpan MENU_ACTION_TIME = new TimeSpan(0, 0, 0, 0, 500);
        #endregion Time Stuff

        #region Enums
        public enum EnemyType { Spider }
        public enum TowerType { Basic }
        public enum TrapType { Basic }
        #endregion Enums

        #region Colors
        public static Color PLACING_COLOR = Color.FromNonPremultiplied(255, 255, 255, 150);
        public static Color PAUSE_GRAY = Color.FromNonPremultiplied(50, 50, 50, 200);
        
        public static Color EffectTransparency(float percent, Color clr)
        {
            return Color.FromNonPremultiplied(clr.R, clr.G, clr.B, (byte)(clr.A * percent));
        }
        #endregion Colors
    }
}

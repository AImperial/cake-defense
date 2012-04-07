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
    class Enemy:GameObject
    {
        #region Attributes
        private Path path;
        private int currentTile;
        private TimeSpan time; // hold time when anm action started.

        private bool spawning;
        private float transparency;
        #endregion Attributes

        #region Constructor
        public Enemy(int sh, int dd, int speed, Path path, int w, int h, SpriteBatch spB, Color c, Texture2D t)
            : base(sh, dd, speed, 0, 0, w, h, spB, c, t)
        {
            isActive = false;
            this.path = path;
            Center = path.Start.Center;
            currentTile = 0;

            transparency = 0;
            rotation = RIGHT;
            spawning = true;
        }
        #endregion Constructor

        #region Properties

        #endregion Properties

        #region Methods

        #region Move
        public void Move(GameTime gameTime)
        {
            if (spawning == false)
            {
                MoveBy(speed * Var.GAME_SPEED); // move the enemy (properly)

                // check attack stuff/etc here
            }
            else
            {
                Spawning(gameTime);
            }
        }

        private void MoveBy(int num)
        {
            if (path.InRange(currentTile + 1))
            {
                if (path.GetNextTile(currentTile).Center.X < Center.X)
                {
                    // Encase Rounding a corner
                    if (Center.X - num < path.GetNextTile(currentTile).Center.X)
                    {
                        currentTile++;
                        // How far off it went over center
                        int distOff = (int)(path.GetNextTile(currentTile).Center.X - Center.X);
                        X -= num - distOff;
                        MoveBy(distOff);
                    }
                    // Otherwise move normally
                    else
                    {
                        X -= num;
                        if (Center.X == path.GetNextTile(currentTile).Center.X)
                            currentTile++;
                    }
                }
                else if (path.GetNextTile(currentTile).Center.Y < Center.Y)
                {
                    // Encase Rounding a corner
                    if (Center.Y - num < path.GetNextTile(currentTile).Center.Y)
                    {
                        currentTile++;
                        // How far off it went over center
                        int distOff = (int)(path.GetNextTile(currentTile).Center.Y - Center.Y);
                        Y -= num - distOff;
                        MoveBy(distOff);
                    }
                    // Otherwise move normally
                    else
                    {
                        Y -= num;
                        if (Center.Y == path.GetNextTile(currentTile).Center.Y)
                            currentTile++;
                    }
                }
                else if (path.GetNextTile(currentTile).Center.X > Center.X)
                {
                    // Encase Rounding a corner
                    if (Center.X + num > path.GetNextTile(currentTile).Center.X)
                    {
                        currentTile++;
                        // How far off it went over center
                        int distOff = (int)(Center.X - path.GetNextTile(currentTile).Center.X);
                        X -= num - distOff;
                        MoveBy(distOff);
                    }
                    // Otherwise move normally
                    else
                    {
                        X += num;
                        if (Center.X == path.GetNextTile(currentTile).Center.X)
                            currentTile++;
                    }
                }
                else if (path.GetNextTile(currentTile).Center.Y > Center.Y)
                {
                    // Encase Rounding a corner
                    if (Center.Y + num > path.GetNextTile(currentTile).Center.Y)
                    {
                        currentTile++;
                        // How far off it went over center
                        int distOff = (int)(Center.Y - path.GetNextTile(currentTile).Center.Y);
                        Y -= num - distOff;
                        MoveBy(distOff);
                    }
                    // Otherwise move normally
                    else
                    {
                        Y += num;
                        if (Center.Y == path.GetNextTile(currentTile).Center.Y)
                            currentTile++;
                    }
                }
            }
        }
        #endregion Move

        public void Start(GameTime gameTime)
        {
            isActive = true;
            transparency = 0;
            rotation = 0;
            spawning = true;
            time = new TimeSpan(0, 0, 0, 0, (int)gameTime.TotalGameTime.TotalMilliseconds);
        }

        /// <summary> Called in Move. Controls enemy's behavior when spawning. </summary>
        public void Spawning(GameTime gameTime)
        {
            if ((time + Var.SPAWN_TIME).TotalMilliseconds > gameTime.TotalGameTime.TotalMilliseconds)
            {
                float percComplete = Var.TimePercentTillComplete(time, Var.SPAWN_TIME, gameTime);
                transparency = percComplete;
                rotation = (float)(Var.SPAWN_SPINS * (Math.PI * 2) * percComplete);
            }
            else
            {
                transparency = 100;
                rotation = RIGHT;
                time = TimeSpan.Zero;
                spawning = false;
                isActive = true;
            }
        }

        #endregion Methods

        #region Draw
        public void Draw(GameTime gameTime)
        {
            if (isActive)
            {
                #region Spawning
                if (spawning)
                {
                    // This allow to rotate image by center
                    Rectangle drawOff = Rectangle;
                    drawOff.X += Width / 2;
                    drawOff.Y += Height / 2;

                    sprBtch.Draw(
                        Texture,
                        drawOff,
                        new Rectangle(0 + (16 * ((int)(gameTime.TotalGameTime.TotalMilliseconds / 500) % 3)), 0, 16, 16),
                        Color,//Var.EffectTransparency(transparency, color),
                        rotation,
                        new Vector2(8),
                        SpriteEffects.None,
                        0
                    );
                }
                #endregion Spawning
                else
                {
                    Rectangle drawOff = Rectangle;
                    drawOff.X += Width / 2;
                    drawOff.Y += Height / 2;

                    sprBtch.Draw(
                        Texture,
                        drawOff,
                        new Rectangle(0 + (16 * ((int)(gameTime.TotalGameTime.TotalMilliseconds / 500) % 3)),0, 16, 16),
                        Color,
                        rotation,
                        new Vector2(8),
                        SpriteEffects.None,
                        0
                    );
                }
            }
        }
        #endregion Draw
    }
}

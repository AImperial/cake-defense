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
        private TimeSpan time; // hold time when an action started.

        private bool spawning, despawning;
        private float transparency;
        #endregion Attributes

        #region Constructor
        public Enemy(ImageObject imageObject, int health, int damage, int speed, Path path)
            : base(imageObject, health, damage, speed)
        {
            IsActive = false;
            this.path = path;
            Image.Center = Center = path.Start.Center;
            currentTile = 0;

            transparency = 0;
            Image.Rotation = ImageObject.RIGHT;
            spawning = true;
        }
        #endregion Constructor

        #region Properties

        #endregion Properties

        #region Methods

        #region Move
        public void Move(GameTime gameTime)
        {
            if (IsActive)
            {
                if (spawning == false && despawning == false)
                {
                    MoveBy(Speed * Var.GAME_SPEED); // move the enemy (properly)

                    if (path.GetTile(currentTile) == path.End)
                    {
                        despawning = true;
                        time = new TimeSpan(0, 0, 0, 0, (int)gameTime.TotalGameTime.TotalMilliseconds);
                        Move(gameTime);
                        return;
                    }
                    // check attack stuff/etc here
                }
                else if (spawning == true)
                {
                    Spawning(gameTime);
                }
                else if (despawning == true)
                {
                    Despawning(gameTime);
                }
            }
        }

        private void MoveBy(int num)
        {
            if (path.InRange(currentTile + 1))
            {
                // Move Left
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
                        Image.Rotation = ImageObject.LEFT;
                        X -= num;
                        if (Center.X == path.GetNextTile(currentTile).Center.X)
                            currentTile++;
                    }
                }
                // Move Up
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
                        Image.Rotation = ImageObject.UP;
                        Y -= num;
                        if (Center.Y == path.GetNextTile(currentTile).Center.Y)
                            currentTile++;
                    }
                }
                // Move Right
                else if (path.GetNextTile(currentTile).Center.X > Center.X)
                {
                    // Encase Rounding a corner
                    if (Center.X + num > path.GetNextTile(currentTile).Center.X)
                    {
                        currentTile++;
                        // How far off it went over center
                        int distOff = (int)(Center.X - path.GetNextTile(currentTile).Center.X);
                        X += num - distOff;
                        MoveBy(distOff);
                    }
                    // Otherwise move normally
                    else
                    {
                        Image.Rotation = ImageObject.RIGHT;
                        X += num;
                        if (Center.X == path.GetNextTile(currentTile).Center.X)
                            currentTile++;
                    }
                }
                // Move Down
                else if (path.GetNextTile(currentTile).Center.Y > Center.Y)
                {
                    // Encase Rounding a corner
                    if (Center.Y + num > path.GetNextTile(currentTile).Center.Y)
                    {
                        currentTile++;
                        // How far off it went over center
                        int distOff = (int)(Center.Y - path.GetNextTile(currentTile).Center.Y);
                        Y += num - distOff;
                        MoveBy(distOff);
                    }
                    // Otherwise move normally
                    else
                    {
                        Image.Rotation = ImageObject.DOWN;
                        Y += num;
                        if (Center.Y == path.GetNextTile(currentTile).Center.Y)
                            currentTile++;
                    }
                }
            }
        }
        #endregion Move

        #region Spawning / Despawning Stuff
        public void Start(GameTime gameTime)
        {
            IsActive = true;
            transparency = 0;
            Image.Rotation = ImageObject.RIGHT;
            spawning = true;
            time = new TimeSpan(0, 0, 0, 0, (int)gameTime.TotalGameTime.TotalMilliseconds);
        }

        /// <summary> Called in Move. Controls enemy's behavior when spawning. </summary>
        private void Spawning(GameTime gameTime)
        {
            if ((time + Var.SPAWN_TIME).TotalMilliseconds > gameTime.TotalGameTime.TotalMilliseconds)
            {
                float percComplete = Var.TimePercentTillComplete(time, Var.SPAWN_TIME, gameTime);
                transparency = percComplete;
                Image.Rotation = (float)(Var.SPAWN_SPINS * (Math.PI * 2) * percComplete);
            }
            else
            {
                transparency = 100;
                Image.Rotation = ImageObject.RIGHT;
                time = TimeSpan.Zero;
                spawning = false;
                IsActive = true;
            }
        }

        /// <summary> Called in Move. Controls enemy's behavior when despawning. </summary>
        private void Despawning(GameTime gameTime)
        {
            if ((time + Var.DESPAWN_TIME).TotalMilliseconds > gameTime.TotalGameTime.TotalMilliseconds)
            {
                transparency = 1 - Var.TimePercentTillComplete(time, Var.DESPAWN_TIME, gameTime);
            }
            else
            {
                transparency = 100;
                Image.Rotation = ImageObject.RIGHT;
                time = TimeSpan.Zero;
                despawning = false;
                IsActive = false;

                // something here handling getting cake to the end
            }
        }
        #endregion Spawning / Despawning Stuff

        #endregion Methods

        #region Draw
        public void Draw(GameTime gameTime)
        {
            if (IsActive)
            {
                #region Spawning / Despawning
                if (spawning || despawning)
                {
                    Color tempColor = Image.Color;
                    Image.Color = Var.EffectTransparency(transparency, Image.Color);
                    base.Draw(gameTime, Var.FRAME_SPEED);
                    Image.Color = tempColor;
                }
                #endregion Spawning / Despawning
                else
                {
                    base.Draw(gameTime, Var.FRAME_SPEED);
                }
            }
        }
        #endregion Draw
    }
}

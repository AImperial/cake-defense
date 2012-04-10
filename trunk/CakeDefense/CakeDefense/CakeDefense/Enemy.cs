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
#endregion Using Statements

namespace CakeDefense
{
    class Enemy:GameObject
    {
        #region Attributes
        private Path path;
        private int currentTile;
        private TimeSpan time; // hold time when an action started.

        private bool spawning, despawning, dying;
        private float transparency, slowEffect;
        #endregion Attributes

        #region Constructor
        public Enemy(ImageObject imageObject, int health, int damage, int speed, Path path)
            : base(imageObject, health, damage, speed)
        {
            IsActive = false;
            this.path = path;
            Image.Center = Center = path.Start.Center;
            currentTile = 0;
            slowEffect = 1;
            transparency = 0;
            Image.Rotation = ImageObject.RIGHT;
            spawning = true;
        }
        #endregion Constructor

        #region Properties
        public Path Path
        {
            get { return path; }

            set { path = value; }
        }

        public bool IsSpawning
        {
            get { return spawning; }
        }

        public bool IsDying
        {
            get { return dying; }
        }

        /// <summary> Ranges from 0-1. One is normal speed. </summary>
        public float SlowEffect
        {
            get { return slowEffect; }

            set { slowEffect = value; if (value < 0) { slowEffect = 0; } if (value > 1) { slowEffect = 1; } }
        }
        #endregion Properties

        #region Methods

        #region Move
        public void Move(TimeSpan gameTime, List<Trap> traps)
        {
            if (IsActive)
            {
                if (spawning == false && despawning == false && IsDying == false)
                {
                    MoveBy(Speed * Var.GAME_SPEED * slowEffect, traps); // move the enemy (properly)

                    if (path.GetTile(currentTile) == path.End)
                    {
                        despawning = true;
                        time = new TimeSpan(gameTime.Ticks);
                        Move(gameTime, traps);
                        return;
                    }

                    KillIfCan(gameTime);
                    // check attack stuff/etc here
                }
                else if (despawning)
                {
                    Despawning(gameTime);
                    KillIfCan(gameTime);
                }
                else if (IsDying)
                {
                    Dying(gameTime);
                }
                else if (spawning)
                {
                    Spawning(gameTime);
                }
            }
        }

        private void MoveBy(float num, List<Trap> traps)
        {
            traps.ForEach(trap => trap.AttackIfCan(this));

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
                        float distOff = num - (Center.X - path.GetTile(currentTile).Center.X);
                        X -= num - distOff;
                        MoveBy(distOff, traps);
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
                        float distOff = num - (Center.Y - path.GetTile(currentTile).Center.Y);
                        Y -= num - distOff;
                        MoveBy(distOff, traps);
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
                        float distOff = num - (path.GetTile(currentTile).Center.X - Center.X);
                        X += num - distOff;
                        MoveBy(distOff, traps);
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
                        float distOff = num - (path.GetTile(currentTile).Center.Y - Center.Y);
                        Y += num - distOff;
                        MoveBy(distOff, traps);
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
        public void Start(TimeSpan gameTime)
        {
            IsActive = true;
            transparency = 0;
            Image.Rotation = ImageObject.RIGHT;
            spawning = true;
            time = new TimeSpan(gameTime.Ticks);
        }

        /// <summary> Called in Move. Controls enemy's behavior when spawning. </summary>
        private void Spawning(TimeSpan gameTime)
        {
            if ((time + TimeSpan.FromTicks(Var.SPAWN_TIME.Ticks / Var.GAME_SPEED)) > gameTime)
            {
                float percComplete = Var.TimePercentTillComplete(time, TimeSpan.FromTicks(Var.SPAWN_TIME.Ticks / Var.GAME_SPEED), gameTime);
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
        private void Despawning(TimeSpan gameTime)
        {
            if ((time + TimeSpan.FromTicks(Var.DESPAWN_TIME.Ticks / Var.GAME_SPEED)) > gameTime)
            {
                transparency = 1 - Var.TimePercentTillComplete(time, TimeSpan.FromTicks(Var.DESPAWN_TIME.Ticks / Var.GAME_SPEED), gameTime);
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

        private void KillIfCan(TimeSpan gameTime)
        {
            if (CurrentHealth <= 0)
            {
                dying = true;
                despawning = false;
                time = new TimeSpan(0, 0, 0, 0, (int)gameTime.TotalMilliseconds);
                Move(gameTime, null);
            }
        }

        private void Dying(TimeSpan gameTime)
        {
            if ((time + TimeSpan.FromTicks(Var.DYING_TIME.Ticks / Var.GAME_SPEED)) > gameTime)
            {
                float percComplete = Var.TimePercentTillComplete(time, TimeSpan.FromTicks(Var.DYING_TIME.Ticks / Var.GAME_SPEED), gameTime);
                transparency = percComplete;
                Image.Rotation = (float)(Var.DEATH_SPINS * (Math.PI * 2) * percComplete);
                Image.Resize = new Vector2(1 - percComplete);
                CenterImage();
            }
            else
            {
                transparency = 100;
                Image.Rotation = ImageObject.RIGHT;
                time = TimeSpan.Zero;
                IsActive = false;
            }
        }
        #endregion Spawning / Despawning Stuff

        #endregion Methods

        #region Draw
        public void Draw(GameTime gameTime)
        {
            if (IsActive)
            {
                #region Spawning / Despawning / Dying
                if (spawning || despawning || IsDying)
                {
                    Color tempColor = Image.Color;
                    Image.Color = Var.EffectTransparency(transparency, Image.Color);
                    base.Draw(gameTime, Var.FRAME_SPEED);
                    Image.Color = tempColor;
                }
                #endregion Spawning / Despawning / Dying
                else
                {
                    base.Draw(gameTime, Var.FRAME_SPEED);
                }
            }
        }
        #endregion Draw
    }
}

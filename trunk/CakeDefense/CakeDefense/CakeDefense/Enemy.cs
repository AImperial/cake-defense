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
        //private TimeSpan time; // hold time when an action started.
        Timer timer;

        private bool spawning, despawning, dying;
        private float transparency, slowEffect;
        #endregion Attributes

        #region Constructor
        public Enemy(ImageObject imageObject, int health, int damage, float speed, Path path)
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
            timer = new Timer(Var.GAME_SPEED);
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
        public void Move(GameTime gameTime, List<Trap> traps)
        {
            if (timer != null)
                timer.Update(gameTime);
            if (IsActive)
            {
                if (spawning == false && despawning == false && IsDying == false)
                {
                    MoveBy(Speed * Var.GAME_SPEED * slowEffect, traps); // move the enemy (properly)

                    if (path.GetTile(currentTile) == path.End)
                    {
                        despawning = true;
                        timer.Start(gameTime, Var.DESPAWN_TIME);
                        Move(gameTime, traps);
                        return;
                    }

                    KillIfCan(gameTime);
                    // check attack stuff/etc here
                }
                else if (despawning)
                {
                    Despawning();
                    KillIfCan(gameTime);
                }
                else if (IsDying)
                {
                    Dying();
                }
                else if (spawning)
                {
                    Spawning();
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
        public void Start(GameTime gameTime)
        {
            IsActive = true;
            transparency = 0;
            Image.Rotation = ImageObject.RIGHT;
            spawning = true;
            timer.Start(gameTime, Var.SPAWN_TIME);
        }

        /// <summary> Called in Move. Controls enemy's behavior when spawning. </summary>
        private void Spawning()
        {
            if (timer.Finished == false)
            {
                transparency = timer.Percent;
                Image.Rotation = (float)(Var.SPAWN_SPINS * (Math.PI * 2) * timer.Percent);//percComplete);
            }
            else
            {
                transparency = 100;
                Image.Rotation = ImageObject.RIGHT;
                timer.End();
                spawning = false;
                IsActive = true;
            }
        }

        /// <summary> Called in Move. Controls enemy's behavior when despawning. </summary>
        private void Despawning()
        {
            if (timer.Finished == false)
            {
                transparency = 1 - timer.Percent;
            }
            else
            {
                transparency = 100;
                Image.Rotation = ImageObject.RIGHT;
                timer.End();
                despawning = false;
                IsActive = false;

                // something here handling getting cake to the end
            }
        }

        private void KillIfCan(GameTime gameTime)
        {
            if (CurrentHealth <= 0)
            {
                dying = true;
                despawning = false;
                timer.Start(gameTime, Var.DYING_TIME);
                Move(gameTime, null);
            }
        }

        private void Dying()
        {
            if (timer.Finished == false)
            {
                transparency = timer.Percent;
                Image.Rotation = (float)(Var.DEATH_SPINS * (Math.PI * 2) * timer.Percent);
                Image.Resize = new Vector2(1 - timer.Percent);
                CenterImage();
            }
            else
            {
                transparency = 100;
                Image.Rotation = ImageObject.RIGHT;
                timer.End();
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

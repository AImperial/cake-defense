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
    class Tower:GameObject
    {
        #region Attributes
        protected bool placing, canFire;
        protected Tile_Tower occupiedTile;
        private Stopwatch timer = new Stopwatch();
        private int cost;
        private List<Bullet> bullets;
        #endregion Attributes

        #region Constructor
        public Tower(int health, int co, int damage, int speed, int w, int h, SpriteBatch spB, Color c, Texture2D t)
            : base(t, 0, 0, w, h, spB, health, damage, speed)
        {
            Image.Color = c;
            canFire = true; placing = true;
            cost = co;
            bullets = new List<Bullet>();
        }
        #endregion Constructor

        #region Properties
        public bool Placing
        {
            get { return placing; }
            set { placing = value; }
        }

        public Tile_Tower OccupiedTile
        {
            get { return occupiedTile; }
            set { occupiedTile = value; }
        }

        public bool CanFire
        {
            get { return canFire; }
            set { canFire = value; }
        }

        public int Cost
        {
            get { return cost; }
            set { cost = value; }
        }
        #endregion Properties

        #region Methods
        public void Place(Tile_Tower tile)
        {
            timer.Start();
            placing = false;
            occupiedTile = tile;
            occupiedTile.OccupiedBy = this;
            Center = tile.Center;
            fire(Image.Texture);
        }
        public void fire(Texture2D texture)
        {
            Random rand = new Random();
            bullets.Add(new Bullet(rand.Next(8), (int)Center.X, (int)Center.Y, texture));
        }

        public bool isDead()
        {
            if (CurrentHealth <= 0)
            {
                return true;
            }
            return false;
        }

        public void takeDamage()
        {
            CurrentHealth--;
        }
        #endregion Methods

        #region Draw
        public override void Draw()
        {
            if (IsActive)
            {
                #region Placing
                if (placing)
                {
                    Color tempColor = Image.Color;
                    Image.Color = Var.PLACING_TOWER_COLOR;
                    base.Draw();
                    Image.Color = tempColor;
                }
                #endregion Placing

                else
                {
                    base.Draw();
                    if (bullets.Count > 0)
                    {
                        if (timer.ElapsedMilliseconds >= 500)
                        {
                            timer.Restart();
                            fire(Image.Texture);
                        }

                        foreach (Bullet bullet in bullets)
                        {
                            bullet.draw(Image.SpriteBatch);
                            bullet.move();
                        }

                        if (bullets.Count > 10)
                        {
                            bullets.RemoveRange(0, 1);
                        }
                    }
                }
            }
        }
        #endregion Draw
    }
}

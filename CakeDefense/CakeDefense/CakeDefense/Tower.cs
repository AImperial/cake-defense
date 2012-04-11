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
        private Stopwatch timer;
        private Random rand;
        private int cost;
        private List<Bullet> bullets;
        private Texture2D bulletTexture;
        #endregion Attributes

        #region Constructor
        public Tower(int health, int co, int damage, float speed, int w, int h, SpriteBatch spB, Texture2D t, Texture2D bt)
            : base(t, 0, 0, w, h, spB, health, damage, speed)
        {
            Image.Color = Color.Blue;
            canFire = true; placing = true;
            timer = new Stopwatch();
            rand = new Random();
            cost = co;
            bulletTexture = bt;
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
            Fire();
        }

        public void Fire()
        {
            if (placing == false && IsActive)
            {
                if (timer.ElapsedMilliseconds >= 500)
                {
                    timer.Restart();
                    bullets.Add(new Bullet(rand.Next(8), Damage, Var.BULLET_SPEED, (int)Center.X, (int)Center.Y, Var.BULLET_SIZE, Var.BULLET_SIZE, bulletTexture, Image.SpriteBatch));
                }

                bullets.ForEach(bullet => bullet.Move());

                if (bullets.Count > 10)
                {
                    bullets.RemoveRange(0, 1);
                }
            }
        }

        public void CheckCollision(GameObject GO)
        {
            if (GO is Enemy)
            {
                Enemy curEnemy = (Enemy)GO;
                for (int i = 0; i < bullets.Count; i++)
                {
                    if (bullets[i].IsActive)
                    {
                        if (curEnemy.Rectangle.Intersects(bullets[i].Rectangle) && curEnemy.IsActive && curEnemy.IsSpawning == false)
                        {
                            curEnemy.CurrentHealth -= Damage;
                            bullets.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }

        public bool isDead()
        {
            if (CurrentHealth <= 0)
            {
                return true;
            }
            return false;
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
                    Image.Color = Var.PLACING_COLOR;
                    base.Draw();
                    Image.Color = tempColor;
                }
                #endregion Placing

                else
                {
                    base.Draw();
                    bullets.ForEach(bullet => bullet.Draw());
                }
            }
        }
        #endregion Draw
    }
}

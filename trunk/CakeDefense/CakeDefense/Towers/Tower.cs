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
    // This class should never be created, rather only children classes (thus abstract)
    abstract class Tower:GameObject
    {
        #region Attributes
        protected bool placing, canFire;
        protected Tile_Tower occupiedTile;
        protected Stopwatch timer;
        protected Random rand;
        protected int cost, upgradeLevel;
        protected float fireRadius, bulletSpeed;
        protected List<Bullet> bullets;
        protected Texture2D bulletTexture;
        protected Var.TowerType type;

        public enum Attacktype { None, Fastest, Slowest, Strongest, Weakest, First, Last }
        protected Attacktype attackType;
        #endregion Attributes

        #region Constructor
        public Tower(float fireRadius, int health, int cost, int damage, float speed, float bSpeed, int w, int h, SpriteBatch spB, Texture2D t, Texture2D bt, Attacktype attackType)
            : base(t, 0, 0, w, h, spB, health, damage, speed)
        {
            this.attackType = attackType;
            this.fireRadius = fireRadius;
            bulletSpeed = bSpeed;
            canFire = true; placing = true;
            timer = new Stopwatch();
            rand = new Random();
            this.cost = cost;
            bulletTexture = bt;
            bullets = new List<Bullet>();
            Image.Transparency = Var.PLACING_TRANSPARENCY;
            upgradeLevel = 0;
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

        public int UpgradeLevel
        {
            get { return upgradeLevel; }
            set { upgradeLevel = value; }
        }

        public float BulletSpeed
        {
            get { return bulletSpeed; }
            set { bulletSpeed = value; }
        }

        public float FireRadius
        {
            get { return fireRadius; }
            set { fireRadius = value; }
        }

        public Var.TowerType Type
        {
            get { return type; }
            set { type = value; }
        }

        public Attacktype AttackType
        {
            get { return attackType; }
            set { attackType = value; }
        }
        #endregion Properties

        #region Methods
        public abstract void Fire(List<Enemy> enemies);

        public void Place(Tile_Tower tile)
        {
            timer.Start();
            placing = false;
            occupiedTile = tile;
            occupiedTile.OccupiedBy = this;
            Center = tile.Center;
            Image.Transparency = 100;
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
                        if (curEnemy.Rectangle.Intersects(bullets[i].Rectangle))
                        {
                            curEnemy.Hit(Damage);
                            bullets.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }

        public int SellCost()
        {
            return (int)(cost * ((float)CurrentHealth / (float)StartHealth) / 2f);
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
                base.Draw();
                bullets.ForEach(bullet => bullet.Draw());
            }
        }
        #endregion Draw
    }
}

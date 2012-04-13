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
        protected Stopwatch timer;
        protected Random rand;
        protected int cost;
        protected float fireRadius, bulletSpeed;
        protected List<Bullet> bullets;
        protected Texture2D bulletTexture;
        protected Var.TowerType type;

        public enum Attacktype { None, Fastest, Slowest, Strongest, Weakest }
        protected Attacktype attackType;
        #endregion Attributes

        #region Constructor
        public Tower(float fireRadius, int health, int cost, int damage, float speed, float bSpeed, int w, int h, SpriteBatch spB, Texture2D t, Texture2D bt, Attacktype attackType)
            : base(t, 0, 0, w, h, spB, health, damage, speed)
        {
            type = Var.TowerType.Basic;
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
        public void Place(Tile_Tower tile)
        {
            timer.Start();
            placing = false;
            occupiedTile = tile;
            occupiedTile.OccupiedBy = this;
            Center = tile.Center;
            Image.Transparency = 100;
        }

        public void Fire(List<Enemy> enemies)
        {
            if (placing == false && IsActive)
            {
                if (timer.ElapsedMilliseconds >= Speed / Var.GAME_SPEED)
                {
                    float fireAngle = -1000; // random number to make sure it should fire.
                    List<Enemy> enemiesInRange = new List<Enemy>();
                    #region Get Enemies In Range
                    foreach (Enemy enemy in enemies)
                    {
                        if (enemy.IsDying == false && enemy.IsSpawning == false && fireRadius >= Vector2.Distance(enemy.Center, Center))
                        {
                            enemiesInRange.Add(enemy);
                        }
                    }
                    #endregion Get Enemies In Range

                    #region Attack based on AttackType
                    if (enemiesInRange.Count > 0)
                    {
                        // Holds an enemy until it knows what one to attack (ex: if looking for strongest in-range enemies)
                        Enemy enemyToAttack = enemiesInRange.First(); // To have something to check against

                        switch (attackType)
                        {
                            #region Attacktype.None
                            case Attacktype.None:
                                enemyToAttack = enemiesInRange[rand.Next(enemiesInRange.Count)];
                                break;
                            #endregion Attacktype.None

                            #region Attacktype.Fastest
                            case Attacktype.Fastest:
                                foreach (Enemy enemy in enemiesInRange)
                                {
                                    if (enemy.Speed > enemyToAttack.Speed)
                                        enemyToAttack = enemy;
                                }
                                break;
                            #endregion Attacktype.Fastest

                            #region Attacktype.Slowest
                            case Attacktype.Slowest:
                                foreach (Enemy enemy in enemiesInRange)
                                {
                                    if (enemy.Speed < enemyToAttack.Speed)
                                        enemyToAttack = enemy;
                                }
                                break;
                            #endregion Attacktype.Slowest

                            #region Attacktype.Strongest
                            case Attacktype.Strongest:
                                foreach (Enemy enemy in enemiesInRange)
                                {
                                    if (enemy.CurrentHealth > enemyToAttack.CurrentHealth)
                                        enemyToAttack = enemy;
                                }
                                break;
                            #endregion Attacktype.Strongest

                            #region Attacktype.Weakest
                            case Attacktype.Weakest:
                                foreach (Enemy enemy in enemiesInRange)
                                {
                                    if (enemy.CurrentHealth < enemyToAttack.CurrentHealth)
                                        enemyToAttack = enemy;
                                }
                                break;
                            #endregion Attacktype.Weakest
                        }
                        fireAngle = enemyToAttack.CheckWhereIWillBe(this); // attacks enemy chosen
                    }
                    #endregion Attack based on AttackType

                    if (fireAngle != -1000)
                    {
                        timer.Restart();
                        bullets.Add(new Bullet(fireAngle, Damage, bulletSpeed, (int)Center.X, (int)Center.Y, Var.BULLET_SIZE, Var.BULLET_SIZE, bulletTexture, Image.SpriteBatch));
                    }
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

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
    class Tower_Basic:Tower
    {
        #region Constructor
        public Tower_Basic(float fireRadius, int health, int cost, int damage, float speed, float bSpeed, int w, int h, SpriteBatch spB, Texture2D t, Texture2D bt, Attacktype attackType)
            : base(fireRadius, health, cost, damage, speed, bSpeed, w, h, spB, t, bt, attackType)
        {
            type = Var.TowerType.Basic;
        }
        #endregion Constructor

        #region Methods

        public override void Fire(List<Enemy> enemies)
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
        #endregion Methods
    }
}

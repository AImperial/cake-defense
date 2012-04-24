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
    class Tower_Shock:Tower
    {
        #region Constructor
        public Tower_Shock(float fireRadius, int health, int cost, int damage, float speed, float bSpeed, int w, int h, SpriteBatch spB, Texture2D t, Texture2D bt, Attacktype attackType)
            : base(fireRadius, health, cost, damage, speed, bSpeed, w, h, spB, t, bt, attackType)
        {
            type = Var.TowerType.Shock;
        }
        #endregion Constructor

        #region Methods
        public override void Fire(List<Enemy> enemies)
        {
            if (placing == false && IsActive)
            {
                if (timer.ElapsedMilliseconds >= Speed / Var.GAME_SPEED)
                {
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

                    #region Attack
                    if (enemiesInRange.Count > 0)
                    {
                        foreach (Enemy enemy in enemiesInRange)
                        {
                            if(timer.ElapsedMilliseconds % 20== 0)
                                enemy.Hit(Damage);
                        }
                    }
                    #endregion Attack
                }
            }
        }
        #endregion Methods
    }
}

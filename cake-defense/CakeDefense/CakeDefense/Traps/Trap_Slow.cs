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
    class Trap_Slow:Trap
    {
        #region Constructor
        public Trap_Slow(int health, int damage, int cost, ImageObject io, Texture2D healthTex)
            :base(health, damage, cost, io, healthTex)
        {
            type = Var.TrapType.Slow;
        }
        #endregion Constructor

        #region Methods
        public override bool AttackIfCan(Enemy enemy, GameTime gameTime)
        {
            if (enemy.Center == Center && IsActive)
            {
                CurrentHealth--;
                enemy.SlowEffect *= 1 - (1f / Damage);
                healthBar.Show(gameTime);
                if (CurrentHealth <= 0)
                    IsActive = false;
            }
            return false;
        }
        #endregion Methods
    }
}

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
    class Trap_Zapper:Trap
    {
        #region Attributes
        private bool isClicked = false;
        #endregion Attributes

        #region Constructor
        public Trap_Zapper(int health, int damage, int cost, ImageObject io, Texture2D healthTex)
            :base(health, damage, cost, io, healthTex)
        {
            type = Var.TrapType.Zapper;
            Image.CutOut = new Rectangle(0, 0, 30, 30);
            Image.TotalFrames = 2;
        }
        #endregion Constructor

        #region Properties
        public bool IsClicked
        {
            get { return isClicked; }
            set { isClicked = value; }
        }
        #endregion Properties

        #region Methods
        public override bool AttackIfCan(Enemy enemy, GameTime gameTime)
        {
            if (IsActive && IsClicked)
            {
                CurrentHealth--;
                if (enemy.Point.X > this.Point.X - 120 && enemy.Point.X < this.Point.X + 120)
                    if (enemy.Point.Y > this.Point.Y - 120 && enemy.Point.Y < this.Point.Y + 120)
                        enemy.Hit(Damage);
                healthBar.Show(gameTime);
                if (CurrentHealth <= 0)
                {
                    IsClicked = false;
                    IsActive = false;
                }
            }
            return false;
        }
        #endregion Methods

        #region Draw
        public override void Draw(GameTime gameTime)
        {
            if(IsClicked)
            {
                base.Draw(gameTime, Var.FRAME_SPEED);
            }
            else
            {
                base.Draw();
            }

            healthBar.Update(gameTime, CurrentHealth, Center.X, Y);
            healthBar.Draw();
        }
        #endregion Draw
    }
}

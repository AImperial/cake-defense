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
    class Bullet:GameObject
    {
        #region Attributes
        #endregion Attributes

        #region Constructor
        public Bullet(float dir, int dmg, float sp, int x, int y, int w, int h, Texture2D texture, SpriteBatch sprite)
            :base(new ImageObject(texture, x, y, w, h, sprite), 0, dmg, sp)
        {
            IsActive = true;
            Direction = dir;
        }
        #endregion Constructor

        #region Properties

        #endregion Properties

        #region Methods
        public void Move()
        {
            X += (float)Math.Cos(Direction) * (Speed * Var.GAME_SPEED);
            Y += (float)Math.Sin(Direction) * (Speed * Var.GAME_SPEED);
        }
        #endregion Methods
    }
}
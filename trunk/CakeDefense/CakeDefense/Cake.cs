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
    class Cake:GameObject
    {
        #region Attributes
        SpriteBatch sprite;
        SpriteFont spriteF;
        #endregion Attributes

        #region Constructor
        public Cake(int health, int x, int y, int w, int h, SpriteBatch spB, SpriteFont spF, Texture2D t)
            : base(t, x, y, w, h, spB, health, 0, 0)
        {
            sprite = spB;
            spriteF = spF;
        }
        #endregion Constructor

        #region Properties

        #endregion Properties

        #region Draw
        public override void Draw()
        {
            if (IsActive)
            {
                base.Draw();
                if(this.CurrentHealth < this.StartHealth)
                    sprite.DrawString(spriteF, "Health Down! (" + this.CurrentHealth + "left)", new Vector2(X + (Width - spriteF.MeasureString("Health Down! (" + this.CurrentHealth + "left)").X) / 2, Y - 5), Color.Red);
            }
        }
        #endregion Draw
    }
}

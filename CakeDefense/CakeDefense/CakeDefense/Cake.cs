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
        #region Constructor
        public Cake(int health, int x, int y, int w, int h, SpriteBatch spB, Texture2D t)
            : base(t, x, y, w, h, spB, health, 0, 0)
        {

        }
        #endregion Constructor

        #region Draw
        public override void Draw()
        {
            if (IsActive)
            {
                Image.Color = Color.Green;
                base.Draw();
            }
        }
        #endregion Draw
    }
}

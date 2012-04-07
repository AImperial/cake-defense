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
        #region Constructor
        public Tower(int health, int damage, int speed, int x, int y, int w, int h, SpriteBatch spB, Color c, Texture2D t)
            : base(t, x, y, w, h, spB, health, damage, speed)
        {
            Image.Color = c;
        }
        #endregion Constructor
    }
}

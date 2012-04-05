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
        public Cake(int sh, int dd, int sp, int x, int y, int w, int h, SpriteBatch spB, Color c, Texture t)
            : base(sh, dd, sp, x, y, w, h, spB, c, t)
        {

        }
    }
}

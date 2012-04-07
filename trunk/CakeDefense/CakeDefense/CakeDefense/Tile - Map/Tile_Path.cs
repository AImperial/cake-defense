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
    class Tile_Path:Tile
    {
        protected int pathNum;

        public Tile_Path(int x, int y, int w, int h, Point tileNum, int pathType)
            : base(x, y, w, h, tileNum)
        {
            pathNum = pathType;
        }

        #region Properties
        public int Type
        {
            get { return pathNum; }

            set { if (value > 0) { pathNum = value; } }
        }
        #endregion Properties
    }
}

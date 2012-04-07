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
    class Tile:GameObject
    {
        #region Attributes
        protected Point tileNum; // Where the tile is located in tile Array
        protected Tile[] neighbors;
        #endregion Attributes

        #region Constructor
        public Tile(int x, int y, int w, int h, Point tileNum)
            : base()
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
            this.tileNum = tileNum;
            neighbors = new Tile[4];
        }
        #endregion Constructor

        #region Propeties
        /// <summary> Where the tile is located in tile Array </summary>
        public Point TileNum
        {
            get { return tileNum; }
        }

        /// <summary> Holds the 4 neighboring Tiles </summary>
        public Tile[] Neighbors
        {
            get { return neighbors; }

            set { }
        }
        #endregion Propeties
    }
}

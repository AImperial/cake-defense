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
        protected Point tileNum; // Where the tile is located in tile Array
        /// <summary>
        /// This contains references to the for nodes surrounding 
        /// this tile (Up, Down, Left, Right).
        /// </summary>
        protected Tile[] neighbors;

        public Tile(int x, int y, int w, int h, Point tileNum)
            : base(0, 0, 0, x, y, w, h, null, Color.White, null)
        {
            this.tileNum = tileNum;
            neighbors = new Tile[4];
        }

        #region Propeties

        /// <summary> Where the tile is located in tile Array </summary>
        public Point TileNum
        {
            get { return tileNum; }
        }

        public Tile[] Neighbors
        {
            get { return neighbors; }

            set { }
        }

        #endregion Propeties
    }
}

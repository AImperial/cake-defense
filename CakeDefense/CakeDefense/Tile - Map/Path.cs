﻿#region Using Statements
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
    class Path
    {
        #region Attributes
        protected List<Tile> tiles;
        #endregion Attributes

        #region Constructor
        public Path(List<Tile> tilePath)
        {
            tiles = tilePath;
        }
        #endregion Constructor

        #region Properties
        /// <summary> A List of Tiles in order of the Path. </summary>
        public List<Tile> Tiles
        {
            get { return tiles; }
        }

        /// <summary> The first Tile. </summary>
        public Tile Start
        {
            get { return tiles.First(); }
        }

        /// <summary> The last Tile. </summary>
        public Tile End
        {
            get { return tiles.Last(); }
        }

        public int Length
        {
            get { return tiles.Count; }
        }
        #endregion Properties

        #region Methods
        public bool InRange(int num)
        {
            if (num < Length && num >= 0)
                return true;
            else
                return false;
        }

        public Tile GetTile(int num)
        {
            return tiles[num];
        }

        public Tile GetNextTile(int num)
        {
            if (InRange(num + 1))
                return tiles[num + 1];
            else
                return tiles[num];
        }

        public Tile GetPrevTile(int num)
        {
            if (InRange(num - 1))
                return tiles[num - 1];
            else
                return tiles[num];
        }

        public int GetIndex(Tile tile)
        {
            for (int i = 0; i < Length; i++)
            {
                if (tiles[i] == tile)
                    return i;
            }
            return 0;
        }
        #endregion Methods
    }
}

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
        #region Attributes
        protected bool placing;
        protected Tile_Tower occupiedTile;
        #endregion Attributes

        #region Constructor
        public Tower(int health, int damage, int speed, int w, int h, SpriteBatch spB, Color c, Texture2D t)
            : base(t, 0, 0, w, h, spB, health, damage, speed)
        {
            Image.Color = c;
            placing = true;
        }
        #endregion Constructor

        #region Properties

        public bool Placing
        {
            get { return placing; }

            set { placing = value; }
        }

        public Tile_Tower OccupiedTile
        {
            get { return occupiedTile; }

            set { occupiedTile = value; }
        }

        #endregion Properties

        #region Methods
        public void Place(Tile_Tower tile)
        {
            placing = false;
            occupiedTile = tile;
            occupiedTile.OccupiedBy = this;
            Center = tile.Center;
        }
        #endregion Methods

        #region Draw
        public override void Draw()
        {
            if (IsActive)
            {
                #region Placing
                if (placing)
                {
                    Color tempColor = Image.Color;
                    Image.Color = Var.PLACING_TOWER_COLOR;
                    base.Draw();
                    Image.Color = tempColor;
                }
                #endregion Placing
                else
                {
                    base.Draw();
                }
            }
        }
        #endregion Draw
    }
}

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
    class Enemy:GameObject
    {
        #region Attributes
        private List<Tile> path;
        private int currentTile;
        private int speed;
        #endregion Attributes

        #region Constructor
        public Enemy(int sh, int dd, int sp, int x, int y, int w, int h, SpriteBatch spB, Color c, Texture2D t, List<Tile> path, int speed)
            : base(sh, dd, sp, x, y, w, h, spB, c, t)
        {
            this.path = path;
            currentTile = 0;
            this.speed = speed;
        }
        #endregion Constructor

        #region Properties

        #endregion Properties

        #region Methods
        public void Move()
        {
            //check current tile
            //if a valid path tile, move along it towards the next
            //if(tileArray[(int)location.X / 40, (int)location.Y / 40] == 1)
            //round up?
            //rectangle.X++;

            MoveBy(speed); // move the enemy (properly)

            //; cehck attack stuff/etc here
        }

        private void MoveBy(int num)
        {
            if (currentTile + 1 < path.Count)
            {
                if (path[currentTile + 1].Center.X < Center.X)
                {
                    // Encase Rounding a corner
                    if (Center.X - num <= path[currentTile + 1].Center.X)
                    {
                        currentTile++;
                        // How far off it went over center
                        int distOff = (int)(path[currentTile + 1].Center.X - Center.X);
                        X -= num - distOff;
                        MoveBy(distOff);
                    }
                    // Otherwise move normally
                    else
                    {
                        X -= num;
                    }
                }
                else if (path[currentTile + 1].Center.Y < Center.Y)
                {
                    // Encase Rounding a corner
                    if (Center.Y - num <= path[currentTile + 1].Center.Y)
                    {
                        currentTile++;
                        // How far off it went over center
                        int distOff = (int)(path[currentTile + 1].Center.Y - Center.Y);
                        Y -= num - distOff;
                        MoveBy(distOff);
                    }
                    // Otherwise move normally
                    else
                    {
                        Y -= num;
                    }
                }
                else if (path[currentTile + 1].Center.X > Center.X)
                {
                    // Encase Rounding a corner
                    if (Center.X + num >= path[currentTile + 1].Center.X)
                    {
                        currentTile++;
                        // How far off it went over center
                        int distOff = (int)(Center.X - path[currentTile + 1].Center.X);
                        X -= num - distOff;
                        MoveBy(distOff);
                    }
                    // Otherwise move normally
                    else
                    {
                        X += num;
                    }
                }
                else if (path[currentTile + 1].Center.Y > Center.Y)
                {
                    // Encase Rounding a corner
                    if (Center.Y + num >= path[currentTile + 1].Center.Y)
                    {
                        currentTile++;
                        // How far off it went over center
                        int distOff = (int)(Center.Y - path[currentTile + 1].Center.Y);
                        Y -= num - distOff;
                        MoveBy(distOff);
                    }
                    // Otherwise move normally
                    else
                    {
                        Y += num;
                    }
                }
            }
        }
        #endregion Methods
    }
}

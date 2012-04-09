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
#endregion Using Statements

namespace CakeDefense
{
    class Bullet
    {
        #region Attributes
        private int locX, locY, direction, SPEED;
        private bool isActive;
        private Texture2D texture;
        #endregion Attributes

        #region Properties
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        public int LocX
        {
            get { return locX; }
        }
        public int LocY
        {
            get { return locY; }
        }
        public int Direction
        {
            get { return direction; }
        }
        #endregion Properties

        #region Constructor
        public Bullet(int dir, int lx, int ly, Texture2D texture)
        {
            isActive = true;
            direction = dir;
            locX = lx;
            locY = ly;
            SPEED = Var.BULLET_SPEED;
            this.texture = texture;
        }
        #endregion Constructor

        #region Methods
        public void move()
        {
            switch (direction)
            {
                case 0: //North
                    locY -= SPEED;
                    break;
                case 1: //North East
                    locY -= (SPEED / 2);
                    locX += (SPEED / 2);
                    break;
                case 2: //East
                    locX += SPEED;
                    break;
                case 3: //South East
                    locY += (SPEED / 2);
                    locX += (SPEED / 2);
                    break;
                case 4: //South
                    locY += SPEED;
                    break;
                case 5: //South West
                    locY += (SPEED / 2);
                    locX -= (SPEED / 2);
                    break;
                case 6: //West
                    locX -= SPEED;
                    break;
                case 7: //North West
                    locY -= (SPEED / 2);
                    locX -= (SPEED / 2);
                    break;
            }
        }
        #endregion Methods

        public void draw(SpriteBatch spr)
        {
            spr.Draw(texture, new Rectangle(locX, locY, 5, 5), null, Color.Purple);
        }
    }
}
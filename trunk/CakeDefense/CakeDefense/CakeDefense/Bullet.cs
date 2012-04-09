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
    class Bullet:GameObject
    {
        #region Attributes
        private int locX, locY, direction;
        Texture2D tex;
        #endregion Attributes

        #region Properties
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
            IsActive = true;
            direction = dir;
            locX = lx;
            locY = ly;
            Speed = Var.BULLET_SPEED;
            tex = texture;
        }
        #endregion Constructor

        #region Methods
        public void move()
        {
            switch (direction)
            {
                case 0: //North
                    locY -= Speed;
                    break;
                case 1: //North East
                    locY -= (Speed / 2);
                    locX += (Speed / 2);
                    break;
                case 2: //East
                    locX += Speed;
                    break;
                case 3: //South East
                    locY += (Speed / 2);
                    locX += (Speed / 2);
                    break;
                case 4: //South
                    locY += Speed;
                    break;
                case 5: //South West
                    locY += (Speed / 2);
                    locX -= (Speed / 2);
                    break;
                case 6: //West
                    locX -= Speed;
                    break;
                case 7: //North West
                    locY -= (Speed / 2);
                    locX -= (Speed / 2);
                    break;
            }
        }
        #endregion Methods

        #region Draw
        public void draw(SpriteBatch spr)
        {
            spr.Draw(tex, new Rectangle(locX, locY, 5, 5), null, Color.Purple);
        }
        #endregion Draw
    }
}
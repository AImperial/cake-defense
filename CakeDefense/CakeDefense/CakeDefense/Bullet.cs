#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion Using Statements

namespace CakeDefense
{
    class Bullet
    {
        #region Attributes
        private int locX, locY, direction, SPEED;
        private bool isActive;
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
        public Bullet(int dir, int lx, int ly)
        {
            isActive = true;
            direction = dir;
            locX = lx;
            locY = ly;
            SPEED = Var.BULLET_SPEED;
        }
        #endregion Constructo

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
    }
}
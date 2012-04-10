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
        private int direction;
        Texture2D tex;
        #endregion Attributes

        #region Properties
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
            X = lx;
            Y = ly;
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
                    Y -= Speed;
                    break;
                case 1: //North East
                    Y -= (Speed / 2);
                    X += (Speed / 2);
                    break;
                case 2: //East
                    X += Speed;
                    break;
                case 3: //South East
                    Y += (Speed / 2);
                    X += (Speed / 2);
                    break;
                case 4: //South
                    Y += Speed;
                    break;
                case 5: //South West
                    Y += (Speed / 2);
                    X -= (Speed / 2);
                    break;
                case 6: //West
                    X -= Speed;
                    break;
                case 7: //North West
                    Y -= (Speed / 2);
                    X -= (Speed / 2);
                    break;
            }
        }
        #endregion Methods

        #region Draw
        public void draw(SpriteBatch spr)
        {
            spr.Draw(tex, new Rectangle((int)X, (int)Y, 5, 5), null, Color.Purple);
        }
        #endregion Draw
    }
}
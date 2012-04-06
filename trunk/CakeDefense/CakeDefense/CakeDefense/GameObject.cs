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
    public class GameObject
    {
        protected int strtHlth, currHealth, dmgDn, speed; // Max health, current health, damage that the object does, either speed of enemy or speed of bullet
        protected bool isActive; // If the object is currently active
        protected Rectangle location; // The current location and size of the object
        protected SpriteBatch sprBtch; // Objects picture
        protected Color color; // Color shows the type of enemy or tower
        protected Texture pic; // Picture

        #region Constructor
        /// <param name="sh">Max Health</param>
        /// <param name="dd">Damage that the object does</param>
        /// <param name="sp">Either Speed of Enemy or Speed of Bullet</param>
        /// <param name="spB">Objects Picture</param>
        /// <param name="c">Color</param>
        /// <param name="t">Texture (Picture)</param>
        public GameObject(int sh, int dd, int sp, int x, int y, int w, int h, SpriteBatch spB, Color c, Texture t)
        {
            strtHlth = sh;
            currHealth = sh;
            dmgDn = dd;
            isActive = true;
            speed = sp;
            location = new Rectangle(x, y, w, h);
            sprBtch = spB;
            color = c;
            pic = t;
        }
        #endregion Constructor

        #region Properties
        public bool IsActive
        {
            get{ return isActive; }

            set { isActive = value; }
        }

        public Rectangle Rectangle
        {
            get { return location; }
        }
        #endregion Properties

        #region Other Methods
        public void Move(int[,] tileArray)
        {
            //check current tile
            //if a valid path tile, move along it towards the next
            //if(tileArray[(int)location.X / 40, (int)location.Y / 40] == 1)
            //round up?
                location.X++;
        }

        public void TakeDamage(int damage)
        {
            currHealth -= damage;
            if (currHealth <= 0)
                isActive = false;
        }
        #endregion Other Methods
    }
}

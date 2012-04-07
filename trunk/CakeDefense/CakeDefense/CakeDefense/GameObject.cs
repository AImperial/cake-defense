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
        protected Rectangle rectangle; // The current location and size of the object
        protected SpriteBatch sprBtch; // Objects picture
        protected Color color; // Color shows the type of enemy or tower
        protected Texture2D pic; // Picture

        #region Constructor
        /// <param name="sh">Max Health</param>
        /// <param name="dd">Damage that the object does</param>
        /// <param name="sp">Either Speed of Enemy or Speed of Bullet</param>
        /// <param name="spB">Objects Picture</param>
        /// <param name="c">Color</param>
        /// <param name="t">Texture (Picture)</param>
        public GameObject(int sh, int dd, int sp, int x, int y, int w, int h, SpriteBatch spB, Color c, Texture2D t)
        {
            strtHlth = sh;
            currHealth = sh;
            dmgDn = dd;
            isActive = true;
            speed = sp;
            rectangle = new Rectangle(x, y, w, h);
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
            get { return rectangle; }
        }

        public Vector2 Vector
        {
            get { return new Vector2(rectangle.X, rectangle.Y); }

            set { rectangle.X = (int)value.X; rectangle.Y = (int)value.Y; }
        }

        public Texture2D Texture
        {
            get { return pic; }

            set { pic = value; }
        }

        public Color Color
        {
            get { return color; }

            set { color = value; }
        }

        public int X
        {
            get { return rectangle.X; }

            set { rectangle.X = value; }
        }

        public int Y
        {
            get { return rectangle.Y; }

            set { rectangle.Y = value; }
        }

        public int Width
        {
            get { return rectangle.Width; }

            set { rectangle.Width = value; }
        }

        public int Height
        {
            get { return rectangle.Height; }

            set { rectangle.Height = value; }
        }

        public Vector2 Center
        {
            get { return new Vector2(rectangle.X + Width / 2, rectangle.Y + Height / 2); }

            set { rectangle.X = (int)value.X - (Width / 2); rectangle.Y = (int)value.Y - (Height / 2); }
        }
        #endregion Properties

        #region Other Methods

        public void TakeDamage(int damage)
        {
            currHealth -= damage;
            if (currHealth <= 0)
                isActive = false;
        }
        #endregion Other Methods

        #region Draw
        public void Draw()
        {
            if(isActive)
                sprBtch.Draw(Texture, Rectangle, Color);
        }
        #endregion Draw
    }
}

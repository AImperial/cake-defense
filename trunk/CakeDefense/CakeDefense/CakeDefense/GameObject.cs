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
    public class GameObject
    {
        #region Attributes
        public ImageObject Image;
        private Vector2 point;
        private Point size;
        private bool isActive;
        private int strtHlth, currHealth, dmgDn, speed;
        #endregion Attributes

        #region Constructors
        public GameObject()
        {
            Image = new ImageObject();
            point = Vector2.Zero;
            size = new Point(10, 10);
            isActive = true;
            speed = 0;
            Damage = 0;
            strtHlth = currHealth = 0;
        }

        public GameObject(ImageObject io)
        {
            Image = io;
            point = new Vector2(io.X, io.Y);
            size = new Point(io.Width, io.Height);
            isActive = true;
            speed = 0;
            Damage = 0;
            strtHlth = currHealth = 0;
        }

        public GameObject(ImageObject io, int health, int damage, int speed)
        {
            Image = io;
            point = new Vector2(io.X, io.Y);
            size = new Point(io.Width, io.Height);
            isActive = true;
            strtHlth = currHealth = health;
            dmgDn = damage;
            this.speed = speed;
        }

        public GameObject(Texture2D texture, float x, float y, int width, int height, SpriteBatch spriteBatch, int health, int damage, int speed)
        {
            Image = new ImageObject(texture, (int)x, (int)y, width, height, spriteBatch);
            point = new Vector2(x, y);
            size = new Point(width, height);
            isActive = true;
            strtHlth = currHealth = health;
            dmgDn = damage;
            this.speed = speed;
        }

        public GameObject(ImageObject io, float x, float y, int width, int height, int health, int damage, int speed)
            : this(io)
        {
            point = new Vector2(x, y);
            size = new Point(width, height);
            strtHlth = currHealth = health;
            dmgDn = damage;
            this.speed = speed;
        }
        #endregion Constructors

        #region Properties

        #region Position Properties
        public float X
        {
            get { return point.X; }

            set { point.X = value; CenterImage(); }
        }

        public float Y
        {
            get { return point.Y; }

            set { point.Y = value; CenterImage(); }
        }

        public int Width
        {
            get { return size.X; }

            set { size.X = value; }
        }

        public int Height
        {
            get { return size.Y; }

            set { size.Y = value; }
        }

        public Vector2 Point
        {
            get { return point; }

            set { point = value; CenterImage(); }
        }

        public Rectangle Rectangle
        {
            get { return new Rectangle((int)X, (int)Y, Width, Height); }

            set
            {
                X = value.X;
                Y = value.Y;
                size = new Point(value.Width, value.Height);
                CenterImage();
            }
        }

        public Vector2 Center
        {
            get { return new Vector2(X + Width / 2, Y + Height / 2); }

            set
            {
                X = (int)value.X - (Width / 2);
                Y = (int)value.Y - (Height / 2);
                CenterImage();
            }
        }
        #endregion Position Properties

        public bool IsActive
        {
            get { return isActive; }

            set { isActive = value; }
        }

        public float Direction
        {
            get { return Image.Rotation; }

            set { Image.Rotation = value; }
        }

        public int StartHealth
        {
            get { return strtHlth; }

            set { strtHlth = value; }
        }

        public int CurrentHealth
        {
            get { return currHealth; }

            set { currHealth = value; }
        }

        public int Damage
        {
            get { return dmgDn; }

            set { dmgDn = value; }
        }

        public int Speed
        {
            get { return speed; }

            set { speed = value; }
        }

        #endregion Properties

        #region Methods
        public void Move(float x, float y)
        {
            X = x;
            Y = y;

            Image.Center = Center;
        }

        public void CenterImage()
        {
            if (Image != null) {
                Image.Center = Center;
                Image.X = (int)(Image.X + ((Image.Resize.X * Image.Width) - Image.Width) / 2);
                Image.Y = (int)(Image.Y + ((Image.Resize.Y * Image.Height) - Image.Height) / 2);
            }
        }
        #endregion Methods

        #region Draw
        public virtual void Draw()
        {
            if (isActive)
                Image.Draw();
        }

        public virtual void Draw(GameTime gameTime, int frameSpeed)
        {
            if (isActive)
                Image.Draw(gameTime, frameSpeed);
        }
        #endregion Draw

        #region Old Version
        /*
        #region Attributes
        protected int strtHlth, currHealth, dmgDn, speed; // Max health, current health, damage that the object does, either speed of enemy or speed of bullet
        protected bool isActive; // If the object is currently active
        protected Rectangle rectangle; // The current location and size of the object
        protected SpriteBatch sprBtch; // Objects picture
        protected Color color; // Color shows the type of enemy or tower
        protected Texture2D pic; // Picture
        protected float rotation;

        #region Const Directions
        public const float UP = 0f;
        public const float RIGHT = (float)Math.PI / 2;
        public const float DOWN = (float)Math.PI;
        public const float LEFT = (3 * (float)Math.PI) / 2;

        public const float UP_RIGHT = (float)Math.PI / 4;
        public const float DOWN_RIGHT = (3 * (float)Math.PI) / 4;
        public const float DOWN_LEFT = (5 * (float)Math.PI) / 4;
        public const float UP_LEFT = (7 * (float)Math.PI) / 4;
        #endregion Const Directions
        #endregion Attributes

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
            rotation = 0;
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

        public float Rotation
        {
            get { return rotation; }

            set { rotation = value; }
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

        public int CenterX
        {
            get { return (int)Center.X; }
        }

        public int CenterY
        {
            get { return (int)Center.Y; }
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
        public virtual void Draw()
        {
            if(isActive)
                sprBtch.Draw(Texture, Rectangle, Color);
        }
        #endregion Draw
        */
        #endregion Old Version
    }
}

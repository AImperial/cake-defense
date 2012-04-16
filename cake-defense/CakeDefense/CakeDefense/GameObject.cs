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
        private int strtHlth, currHealth, dmgDn;
        private float speed;
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

        public GameObject(ImageObject io, int health, int damage, float speed)
        {
            Image = io;
            point = new Vector2(io.X, io.Y);
            size = new Point(io.Width, io.Height);
            isActive = true;
            strtHlth = currHealth = health;
            dmgDn = damage;
            this.speed = speed;
        }

        public GameObject(Texture2D texture, float x, float y, int width, int height, SpriteBatch spriteBatch, int health, int damage, float speed)
        {
            Image = new ImageObject(texture, (int)x, (int)y, width, height, spriteBatch);
            point = new Vector2(x, y);
            size = new Point(width, height);
            isActive = true;
            strtHlth = currHealth = health;
            dmgDn = damage;
            this.speed = speed;
        }

        public GameObject(ImageObject io, float x, float y, int width, int height, int health, int damage, float speed)
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

        public float Speed
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

        #region Draw Rectangle Outline
        /// <summary> Draws an outline around this GameObject. </summary>
        public void DrawRectangleOutline(int borderSize, Color color, Texture2D blankTex)
        {
            ImageObject.DrawRectangleOutline(Rectangle, borderSize, color, blankTex, Image.SpriteBatch);
        }
        #endregion Draw Rectangle Outline
    }
}

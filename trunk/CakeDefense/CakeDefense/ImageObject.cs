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
/// by Torey J. Scheer
/// Made to make using and drawing text objects in xna 4.0 that much easier.
/// 
/// This class allows you to draw an image, including from a sprite sheet, either as an animation or
/// single image, rotate or scale it, and has methods to check collision between game objects and
/// drawing a rectangle outline.

namespace CakeDefense
{
    /// <summary> Contains most 2D Drawing needs. </summary>
    public class ImageObject
    {
        #region Attributes
        protected SpriteBatch spriteBatch;
        protected Texture2D texture;
        protected int xLoc, yLoc, cutX, cutY, drawXOff, drawYOff;
        protected int width, height, cutOutWidth, cutOutHeight;
        protected Color color;
        protected int totalFrames;
        protected float rotation, transparency;
        protected Vector2 origin, resize;
        protected SpriteEffects effects;

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

        #region Constructors
        /// <summary> Default Constructor. </summary>
        public ImageObject()
        {
            this.texture = null;
            this.spriteBatch = null;
            xLoc = 0; yLoc = 0;
            this.width = 1; this.height = 1;
            cutX = 0; cutY = 0;
            cutOutWidth = 10; cutOutHeight = 10;
            drawXOff = 0; drawYOff = 0;
            totalFrames = 1;
            transparency = 100;
            color = Color.White;
            rotation = 0;
            origin = Vector2.Zero;
            effects = SpriteEffects.None;
            resize = Vector2.One;
        }

        /// <summary> Basic Drawable GameObject. </summary>
        public ImageObject(Texture2D texture, SpriteBatch spriteBatch)
            : this()
        {
            this.texture = texture;
            this.spriteBatch = spriteBatch;

            // This methods calls The default constructor,
            // getting default values for this if this is false.
            if (texture != null)
            {
                width = cutOutWidth = texture.Width;
                height = cutOutHeight = texture.Height;
            }
        }

        /// <summary> Standard Drawable GameObject. </summary>
        public ImageObject(Texture2D texture, int x, int y, int width, int height, SpriteBatch spriteBatch)
            : this(texture, spriteBatch)
        {
            xLoc = x; yLoc = y;
            this.width = width; this.height = height;
        }

        /// <summary> Sets every value of a GameObject to something. </summary>
        public ImageObject(Texture2D texture, int x, int y, int width, int height, int totalFrameNum, int cutOutX, int cutOutY, int cutOutWidth,
            int cutOutHeight, int drawXOff, int drawYOff, Color color, float transparency, float rotation, Vector2 origin, SpriteEffects effects, SpriteBatch spriteBatch)
        {
            this.texture = texture;
            this.spriteBatch = spriteBatch;
            xLoc = x; yLoc = y;
            this.width = width; this.height = height;
            this.cutX = cutOutX; this.cutY = cutOutY;
            this.cutOutWidth = cutOutWidth; this.cutOutHeight = cutOutHeight;
            this.drawXOff = drawXOff; this.drawYOff = drawYOff;
            totalFrames = totalFrameNum;
            this.color = color;
            this.transparency = transparency;
            this.rotation = rotation;
            this.origin = origin;
            this.effects = effects;
            resize = Vector2.One;
        }
        #endregion Constructors

        #region Properties

        #region Position Properties
        public Rectangle Rectangle
        {
            get { return new Rectangle(xLoc, yLoc, width, height); }

            set { xLoc = value.X; yLoc = value.Y; width = value.Width; height = value.Height; }
        }

        /// <summary> The X,Y Coordinate of the GameObject. </summary>
        public Vector2 Point
        {
            get { return new Vector2(xLoc, yLoc); }

            set { xLoc = (int)value.X; yLoc = (int)value.Y; }
        }

        public int X
        {
            get { return xLoc; }

            set { xLoc = value; }
        }

        public int Y
        {
            get { return yLoc; }

            set { yLoc = value; }
        }

        public int Width
        {
            get { return width; }

            set { width = value; }
        }

        public int Height
        {
            get { return height; }

            set { height = value; }
        }

        /// <summary> The Center of the rectangle of this GameObject. </summary>
        public Vector2 Center
        {
            get { return new Vector2(xLoc + Width / 2, yLoc + Height / 2); }

            set { xLoc = (int)value.X - (Width / 2); yLoc = (int)value.Y - (Height / 2); }
        }
        #endregion Position Properties

        #region Image Properties
        public Texture2D Texture
        {
            get { return texture; }

            set { texture = value; }
        }

        /// <summary> Used so that the Camera can draw things to the screen. </summary>
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }

            set { spriteBatch = value; }
        }

        public Color Color
        {
            get { return color; }

            set { color = value; }
        }

        /// <summary> Effects the Image's tranparency. (0-100) (100:visible) </summary>
        public float Transparency
        {
            get { return transparency; }

            set { transparency = value; }
        }

        /// <summary> The X location on the Texture to draw. </summary>
        public int CutX
        {
            get { return cutX; }

            set { cutX = value; }
        }

        /// <summary> The Y location on the Texture to draw. </summary>
        public int CutY
        {
            get { return cutY; }

            set { cutY = value; }
        }

        /// <summary> The Width from XOffSet on the Texture to draw. </summary>
        public int CutOutWidth
        {
            get { return cutOutWidth; }

            set { cutOutWidth = value; }
        }

        /// <summary> The Height from YOffSet on the Texture to draw. </summary>
        public int CutOutHeight
        {
            get { return cutOutHeight; }

            set { cutOutHeight = value; }
        }

        /// <summary> The area on the Texture to draw. </summary>
        public Rectangle CutOut
        {
            get { return new Rectangle(cutX, cutY, cutOutWidth, cutOutHeight); }

            set { if (value == null) { cutX = 0; cutY = 0; cutOutWidth = texture.Width; cutOutHeight = texture.Height; } else { cutX = value.X; cutY = value.Y; cutOutWidth = value.Width; cutOutHeight = value.Height; } }
        }

        /// <summary> Changes where the image is drawn in relation to the X value, WHITOUT changing the X. </summary>
        public int DrawXOff
        {
            get { return drawXOff; }

            set { drawXOff = value; }
        }

        /// <summary> Changes where the image is drawn in relation to the Y value, WHITOUT changing the Y. </summary>
        public int DrawYOff
        {
            get { return drawYOff; }

            set { drawYOff = value; }
        }

        /// <summary> Total Number of Frames in the animation (1 if no animation) - Animations go Left to Right on a SpriteSheet. </summary>
        public int TotalFrames
        {
            get { return totalFrames; }

            set { totalFrames = value; }
        }

        /// <summary> Zero / 2PI starts in the EAST direction </summary>
        public float Rotation
        {
            get { return rotation; }

            set { rotation = value; }
        }

        public Vector2 Origin
        {
            get { return origin; }

            set { origin = value; }
        }

        public SpriteEffects Effects
        {
            get { return effects; }

            set { effects = value; }
        }

        public Vector2 Resize
        {
            get { return resize; }

            set { resize = value; }
        }
        #endregion Image Properties

        #endregion Properties

        #region Methods

        /// <summary> Checks collsion of this GameObject and a Rectangle </summary>
        public bool Intersects(Rectangle rect)
        {
            return Rectangle.Intersects(rect);
        }

        /// <summary> Checks collsion of this GameObject and another GameObject </summary>
        public bool Intersects(ImageObject GO)
        {
            return Rectangle.Intersects(GO.Rectangle);
        }

        /// <summary> Moves the current location of the cutout on the spriteSheet by CutOutWidth/CutOutHeight. </summary>
        public void MoveCutOut(int x, int y)
        {
            CutX += (x * CutOutWidth);
            CutY += (y * CutOutHeight);
        }

        /// <summary> Returns a resized version of this GameObject. </summary>
        /// <returns>A Resized Rectangle.</returns>
        public Rectangle Resized()
        {
            Rectangle resizedRect = Rectangle;
            if (resize != Vector2.One && resize != Vector2.Zero)
            {
                resizedRect.X = xLoc - (int)(((width * resize.X) - width) / 2);
                resizedRect.Y = yLoc - (int)(((height * resize.Y) - height) / 2);
                resizedRect.Width = (int)(width * resize.X);
                resizedRect.Height = (int)(height * resize.Y);
            }
            resizedRect.X += drawXOff;
            resizedRect.Y += drawYOff;
            return resizedRect;
        }

        /// <summary> Allows for easily drawing / rotating an image around it's Center. </summary>
        public void CenterOrigin()
        {
            origin = new Vector2(cutOutWidth / 2, cutOutHeight / 2);
            drawXOff = (int)(Center.X - xLoc);
            drawYOff = (int)(Center.Y - yLoc);
        }

        /// <summary> Returns the color use for Image with transparency attribute added to it. </summary>
        public Color TransparentColor()
        {
            if (transparency == 100)
                return color;
            else
                return Color.FromNonPremultiplied(color.R, color.G, color.B, (byte)(color.A * (transparency / 100)));
        }

        #endregion Methods

        #region Draw
        /// <summary> Basic Draw Method of this GameObject. </summary>
        public virtual void Draw()
        {
            Rectangle position = Resized();
            if (texture != null)
                spriteBatch.Draw(texture, position, CutOut, TransparentColor(), rotation, origin, effects, 0);
        }

        /// <summary> Draws an animation (Left to Right on a SpriteSheet). </summary>
        public virtual void Draw(GameTime gameTime, int frameSpeed)
        {
            Rectangle position = Resized();

            if (texture != null)
            {
                Rectangle cutOutAnimation = CutOut;
                cutOutAnimation.X = cutX + (cutOutWidth * ((int)(gameTime.TotalGameTime.TotalMilliseconds / frameSpeed) % totalFrames));

                spriteBatch.Draw(texture, position, cutOutAnimation, TransparentColor(), rotation, origin, effects, 0);
            }
        }
        #endregion Draw

        #region Draw Rectangle Outline
        /// <summary> Draws an outline around this GameObject. </summary>
        public void DrawRectangleOutline(int borderSize, Color color, Texture2D blankTex)
        {
            DrawRectangleOutline(Rectangle, borderSize, color, blankTex, spriteBatch);
        }

        /// <summary> Draws an outline around a rectangle. </summary>
        /// <param name="rect">The Rectangle To Be Outlined</param>
        public static void DrawRectangleOutline(Rectangle rect, int borderSize, Color color, Texture2D blankTex, SpriteBatch sprite)
        {
            int cB = borderSize;
            if (sprite != null && blankTex != null)
            {
                // Draw the 4 lines based on borderSize -  Top, right, bottom, left
                sprite.Draw(blankTex, new Rectangle(rect.X - cB, rect.Y - cB, rect.Width + (cB * 2), cB), color);
                sprite.Draw(blankTex, new Rectangle(rect.X - cB, rect.Y - cB, cB, rect.Height + (cB * 2)), color);
                sprite.Draw(blankTex, new Rectangle(rect.X + rect.Width, rect.Y - cB, cB, rect.Height + (cB * 2)), color);
                sprite.Draw(blankTex, new Rectangle(rect.X - cB, rect.Y + rect.Height, rect.Width + (cB * 2), cB), color);
            }
        }
        #endregion Draw Rectangle Outline
    }
}

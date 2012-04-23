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
    #region Button
    public class Button : ImageObject
    {
        #region Attributes
        protected TextObject message;
        protected bool focused;
        protected int outLineThickness;
        protected Color outlineColor;
        protected List<Button> childButtons;
        protected ButtonEvent bEvent;
        #endregion Attributes

        #region Constructor
        public Button(Texture2D texture, Vector2 loc, int width, int height, int borderSize, Color borderColor, SpriteBatch sprite, TextObject message, ButtonEvent buttonEvent)
            : base(texture, (int)loc.X, (int)loc.Y, width, height, sprite)
        {
            this.message = message;
            outLineThickness = borderSize;
            outlineColor = borderColor;
            // This centers it on the button (thus initial (message.X & message.Y) are unimportant and for style should be Vector2.Zero
            if (message != null && message.Point == Vector2.Zero)
                CenterText();
            focused = false;
            bEvent = buttonEvent;
        }
        #endregion Constructor

        #region Properties

        public TextObject Message
        {
            get { return message; }

            set { message = value; }
        }

        public bool Focused
        {
            get { return focused; }

            set { focused = value; }
        }

        public new Vector2 Resize
        {
            get { return resize; }

            set { resize = value; if (message != null) { message.Resize = value; CenterText(); } }
        }

        /// <summary> Effects the Image's tranparency. (0-100) (100:visible) </summary>
        public new float Transparency
        {
            get { return transparency; }

            set { transparency = value; if (message != null) { message.Transparency = value; } }
        }

        public List<Button> ChildButtons
        {
            get { return childButtons; }

            set { childButtons = value; }
        }

        #endregion Properties

        #region Methods
        public void CenterText()
        {
            if (message != null)
            {
                message.X = Resized().X + (int)((Resized().Width - (message.Width * resize.X)) / 2);
                message.Y = Resized().Y + (int)((Resized().Height - (message.Height * resize.Y)) / 2);
            }
        }

        public void Click()
        {
            if (bEvent != null)
                bEvent(this);
        }
        #endregion Methods

        #region Draw
        public override void Draw()
        {
            base.Draw();
            if (outLineThickness > 0)
                ImageObject.DrawRectangleOutline(Resized(), outLineThickness, Color.FromNonPremultiplied(outlineColor.R, outlineColor.G, outlineColor.B, (byte)(outlineColor.A * (transparency / 100))), Var.BLANK_TEX, spriteBatch);
            if (message != null)
                message.Draw();
        }
        #endregion Draw
    }
    #endregion Button

    #region TextObject
    public class TextObject : ImageObject
    {
        #region Attributes
        private string message;
        private bool drawCenter;
        private Vector2 location;
        private SpriteFont font;
        #endregion Attributes

        #region Constructor
        public TextObject(string message, Vector2 loc, SpriteFont font, Color color, SpriteBatch sprite)
            : base(null, (int)loc.X, (int)loc.Y, 1, 1, sprite)
        {
            this.message = message;
            drawCenter = true;
            location = loc;
            this.font = font;
            this.color = color;
        }
        #endregion Constructor

        #region Properties

        public string Message
        {
            get { return message; }

            set { message = value; }
        }

        public bool DrawCenter
        {
            get { return drawCenter; }

            set { drawCenter = value; }
        }

        public Vector2 Location
        {
            get { return location; }

            set { location = value; }
        }

        public SpriteFont Font
        {
            get { return font; }

            set { font = value; }
        }

        new public int Width
        {
            get { return (int)(font.MeasureString(message).X); }
        }

        new public int Height
        {
            get { return (int)(font.MeasureString(message).Y); }
        }

        /// <summary> The Center of the text of this TextObject. </summary>
        public new Vector2 Center
        {
            get { return new Vector2(xLoc + Width / 2, yLoc + Height / 2); }

            set { xLoc = (int)value.X - (Width / 2); yLoc = (int)value.Y - (Height / 2); }
        }

        #endregion Properties

        #region Draw
        public override void Draw()
        {
            if (drawCenter == false)
                SpriteBatch.DrawString(font, message, Point, TransparentColor(), 0, Vector2.Zero, resize, SpriteEffects.None, 0);
            else
            {
                int bigWdth = 0;
                string[] parts = message.Split('\n').Select(p => p.Trim()).ToArray();
                foreach (string part in parts)
                {
                    if (font.MeasureString(part).X > bigWdth)
                        bigWdth = (int)(font.MeasureString(part).X * resize.X);
                }
                for (int i = 0; i < parts.Length; i++)
                {
                    Vector2 pos = new Vector2(X + ((bigWdth - font.MeasureString(parts[i]).X * resize.X) / 2), Y + ((font.MeasureString(" ").Y - (font.LineSpacing / 2)) * i * resize.Y));
                    
                    SpriteBatch.DrawString(font, parts[i], pos, TransparentColor(), 0, Vector2.Zero, resize, SpriteEffects.None, 0);
                }
            }
        }
        #endregion Draw
    }
    #endregion TextObject
}

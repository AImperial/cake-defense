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
    class Window:ImageObject
    {
        #region Attributes
        protected bool isActive;
        protected List<Button> buttons;
        protected List<TextObject> textObjects;
        #endregion Attributes

        #region Constructor
        public Window(int x, int y, int w, int h, SpriteBatch spB, Texture2D t, List<Button> buttons, List<TextObject> textObjects)
            : base(t, x, y, w, h, spB)
        {
            isActive = false;

            this.buttons = buttons;
            if (buttons == null)
                this.buttons = new List<Button>();

            this.textObjects = textObjects;
            if (textObjects == null)
                this.textObjects = new List<TextObject>();
        }
        #endregion Constructor

        #region Properties
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public List<Button> Buttons
        {
            get { return buttons; }
            set { buttons = value; }
        }

        public List<TextObject> TextObjects
        {
            get { return textObjects; }
            set { textObjects = value; }
        }
        #endregion Properties

        #region Methods

        #endregion Methods

        #region Draw
        public override void Draw()
        {
            if (IsActive)
            {
                base.Draw();
                buttons.ForEach(button => button.Draw());
                textObjects.ForEach(button => button.Draw());
            }
        }
        #endregion Draw
    }
}

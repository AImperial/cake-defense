﻿#region Using Statements
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
    class HUD
    {
        #region Attributes
        private SpriteBatch spriteBatch;
        private int money, score;
        #endregion Attributes

        #region Constructor
        public HUD(SpriteBatch spriteBatch, int money)
        {
            this.spriteBatch = spriteBatch;
            this.money = Var.START_MONEY;
        }
        #endregion Constructor

        #region Properties

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }

            set { spriteBatch = value; }
        }

        public int Money
        {
            get { return money; }

            set { money = value; }
        }

        public int Score
        {
            get { return score; }

            set { score = value; }
        }

        #endregion Properties

        #region Methods
        /// <summary> Checks to see if you can spend a certain ammount of money (Does NOT subtract it). </summary>
        public bool CanSpendMoney(int spending)
        {
            if (money - spending >= 0)
                return true;
            return false;
        }
        #endregion Methods

        #region Draw
        public void Draw(Texture2D infoBox, SpriteFont font)
        {
            Rectangle moneyInfoBox = new Rectangle(2, 2, 120, 40);
            spriteBatch.Draw(infoBox, moneyInfoBox, Color.DarkKhaki);
            ImageObject.DrawRectangleOutline(moneyInfoBox, 2, Color.DarkGray, infoBox, spriteBatch);
            spriteBatch.DrawString(font, "$" + money, new Vector2(moneyInfoBox.X + (moneyInfoBox.Width - font.MeasureString("$" + money).X) / 2, moneyInfoBox.Y + (moneyInfoBox.Height - font.MeasureString("-").Y) / 2), Color.Black);
        }
        #endregion Draw
    }
}

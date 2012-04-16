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
    class HealthBar
    {
        #region Attributes
        Texture2D texture;
        SpriteBatch spriteBatch;

        Timer timer;
        TimeSpan showTime;
        float startFadingByPerc;
        int originalWidth, heightUpExtra, maxHealth, health;
        Rectangle position;
        Color barColor, capColor;
        #endregion Attributes

        #region Constructor
        public HealthBar(Texture2D texture, int maxHealth, SpriteBatch sprite, int heightUpExtra, TimeSpan showTime, float startFadingByPerc, Color barColor, Color capColor)
        {
            this.texture = texture;
            spriteBatch = sprite;

            timer = new Timer(Var.GAME_SPEED);
            this.maxHealth = maxHealth;
            this.heightUpExtra = heightUpExtra;
            this.showTime = showTime;
            this.startFadingByPerc = startFadingByPerc;
            this.barColor = barColor;
            this.capColor = capColor;

            if (maxHealth <= Var.HEALTHBAR_SIZE_MAX.X)
                originalWidth = maxHealth;
            else
                originalWidth = Var.HEALTHBAR_SIZE_MAX.X;

            position = new Rectangle(0, 0, originalWidth, Var.HEALTHBAR_SIZE_MAX.Y);
        }
        #endregion Constructor

        #region Properties
        public int OriginalWidth
        {
            get { return originalWidth; }

            set { originalWidth = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }

            set { texture = value; }
        }
        #endregion Properties

        #region Methods
        public void Show(GameTime gameTime)
        {
            timer.Start(gameTime, showTime);
        }

        public void Update(GameTime gameTime, int hp, float centerX, float yVal)
        {
            timer.Update(gameTime, Var.GAME_SPEED);
            health = hp;
            position.X = (int)centerX - (originalWidth / 2);
            position.Y = (int)yVal - position.Height - heightUpExtra;
            position.Width = (int)(originalWidth * ((float)hp / maxHealth));
        }

        public void Hide()
        {
            timer.End();
        }
        #endregion Methods

        #region Draw
        public void Draw()
        {
            if (timer.Finished == false)
            {
                Color colorBar = barColor;
                Color colorCaps = capColor;
                int capsWidth = 2;

                if (timer.Percent >= startFadingByPerc)
                {
                    colorBar = Var.EffectTransparency(1 - Timer.GetPercentRelative(startFadingByPerc, timer.Percent, 1f), colorBar);
                    colorCaps = Var.EffectTransparency(1 - Timer.GetPercentRelative(startFadingByPerc, timer.Percent, 1f), colorCaps);
                }

                spriteBatch.Draw(texture, position, colorBar);

                if (originalWidth > position.Height)
                {
                    spriteBatch.Draw(texture, new Rectangle(position.X - capsWidth, position.Y - capsWidth, capsWidth, position.Height + (capsWidth * 2)), colorCaps);
                    spriteBatch.Draw(texture, new Rectangle(position.X, position.Y - capsWidth, position.Height, capsWidth), colorCaps);
                    spriteBatch.Draw(texture, new Rectangle(position.X, position.Y + position.Height, position.Height, capsWidth), colorCaps);

                    spriteBatch.Draw(texture, new Rectangle(position.X + originalWidth, position.Y - capsWidth, capsWidth, position.Height + (capsWidth * 2)), colorCaps);
                    spriteBatch.Draw(texture, new Rectangle(position.X + originalWidth - position.Height, position.Y - capsWidth, position.Height, capsWidth), colorCaps);
                    spriteBatch.Draw(texture, new Rectangle(position.X + originalWidth - position.Height, position.Y + position.Height, position.Height, capsWidth), colorCaps);

                }
                else
                {
                    ImageObject.DrawRectangleOutline(new Rectangle(position.X, position.Y, originalWidth, position.Height), capsWidth, colorCaps, texture, spriteBatch);
                }
            }
        }
        #endregion Draw
    }
}

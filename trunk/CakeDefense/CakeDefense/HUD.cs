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
    class HUD
    {
        #region Attributes
        private SpriteBatch spriteBatch;
        private int money, score;
        private Cake cake;
        private Button moneyDisplay, cakeDisplay, activeMenuDisplay;
        //private List<Button> activeMenuDisplayButtons;
        private Timer menuTimer;
        #endregion Attributes

        #region Constructor
        public HUD(SpriteBatch sprite, int money, Texture2D infoBoxTex, SpriteFont font, Cake cake)
        {
            this.spriteBatch = sprite;
            this.money = money;
            this.cake = cake;

            moneyDisplay = new Button(infoBoxTex, new Vector2(2, 2), 120, 40, 2, Color.DarkGray, sprite, new TextObject("$" + money, Vector2.Zero, font, Color.Black, sprite));
            moneyDisplay.Color = Color.DarkKhaki;
            cakeDisplay = new Button(infoBoxTex, new Vector2(125, 2), 120, 40, 2, Color.DarkGray, sprite, new TextObject("Cake: " + cake.CurrentHealth, Vector2.Zero, font, Color.Black, sprite));
            cakeDisplay.Color = Color.DarkKhaki;

            activeMenuDisplay = new Button(infoBoxTex, new Vector2(Var.TOTAL_WIDTH - 120 - 2, 2), 120, 40, 2, Color.LightCyan, sprite, new TextObject("Menu", Vector2.Zero, font, Color.GhostWhite, sprite));
            activeMenuDisplay.Color = Color.Navy;
            PopulateMenuList();
            menuTimer = new Timer(Var.GAME_SPEED);
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

        public int Health
        {
            get { return cake.CurrentHealth; }
            set { cake.CurrentHealth = value; }
        }

        public Button MenuButton
        {
            get { return activeMenuDisplay; }
        }

        //public List<Button> MenuButtonList
        //{
        //    get { return activeMenuDisplayButtons; }
        //}
        #endregion Properties

        #region Methods

        public void Update(GameTime gameTime)
        {
            moneyDisplay.Message.Message = "$" + money; moneyDisplay.CenterText();
            cakeDisplay.Message.Message = "Cake: " + cake.CurrentHealth;

            menuTimer.Update(gameTime);
            if (menuTimer.Finished == false)
            {
                activeMenuDisplay.ChildButtons[0].Y = activeMenuDisplay.Y + (int)((activeMenuDisplay.Height + 2) * menuTimer.Percent);
                activeMenuDisplay.ChildButtons[0].CenterText();
                for (int i = 1; i < activeMenuDisplay.ChildButtons.Count; i++)
                {
                    activeMenuDisplay.ChildButtons[i].Y = activeMenuDisplay.ChildButtons[i - 1].Y + (int)((activeMenuDisplay.ChildButtons[i - 1].Height + 2) * menuTimer.Percent);
                    activeMenuDisplay.ChildButtons[i].CenterText();
                }

                foreach (Button bttn in activeMenuDisplay.ChildButtons[1].ChildButtons)
                {
                    bttn.Y = activeMenuDisplay.ChildButtons[1].Y + 1;
                    bttn.CenterText();
                    bttn.Message.Color = activeMenuDisplay.Message.Color;
                }
            }

            #region Game Speed Buttons
            List<Button> speedList = activeMenuDisplay.ChildButtons[1].ChildButtons;
            speedList.ForEach(bttn => bttn.Message.Color = activeMenuDisplay.Message.Color);

            if (Var.GAME_SPEED == 1)
            {
                speedList[0].Message.Color = Color.Multiply(speedList[0].Message.Color, .5f);
            }
            else if (Var.GAME_SPEED == 2)
            {
                speedList[1].Message.Color = Color.Multiply(speedList[1].Message.Color, .5f);
            }
            else if (Var.GAME_SPEED == 4)
            {
                speedList[2].Message.Color = Color.Multiply(speedList[2].Message.Color, .5f);
            }
            #endregion Game Speed Buttons
        }

        /// <summary> Checks to see if you can spend a certain ammount of money (Does NOT subtract it). </summary>
        public bool CanSpendMoney(int spending)
        {
            if (money - spending >= 0)
                return true;
            return false;
        }

        private void PopulateMenuList()
        {
            Button parent = activeMenuDisplay;
            parent.ChildButtons = new List<Button>();
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point, parent.Width, parent.Height, 2, Color.LightCyan, parent.SpriteBatch,
                new TextObject("Pause", Vector2.Zero, parent.Message.Font, Color.GhostWhite, parent.SpriteBatch)));
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point, parent.Width, parent.Height, 2, Color.LightCyan, parent.SpriteBatch,
                null));
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point, parent.Width, parent.Height, 2, Color.LightCyan, parent.SpriteBatch,
                new TextObject("Restart", Vector2.Zero, parent.Message.Font, Color.GhostWhite, parent.SpriteBatch)));
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point, parent.Width, parent.Height, 2, Color.LightCyan, parent.SpriteBatch,
                new TextObject("Exit", Vector2.Zero, parent.Message.Font, Color.GhostWhite, parent.SpriteBatch)));

            parent.ChildButtons.ForEach(bttn => bttn.Color = Color.Navy);


            // Adds buttons to button 1's list in menu button's button list (ugg...?)
            parent = activeMenuDisplay.ChildButtons[1];
            int tempWidth = parent.Width / 3;
            parent.ChildButtons = new List<Button>();
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point + Vector2.One, tempWidth - 2, parent.Height - 2, 2, Color.LightCyan, parent.SpriteBatch,
                new TextObject("1x", Vector2.Zero, activeMenuDisplay.Message.Font, Color.GhostWhite, parent.SpriteBatch)));
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point + new Vector2(tempWidth, 0) + Vector2.One, tempWidth, parent.Height - 2, 2, Color.LightCyan, parent.SpriteBatch,
                new TextObject("2x", Vector2.Zero, activeMenuDisplay.Message.Font, Color.GhostWhite, parent.SpriteBatch)));
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point + new Vector2(tempWidth * 2, 0) + Vector2.One, tempWidth, parent.Height - 2, 2, Color.LightCyan, parent.SpriteBatch,
                new TextObject("4x", Vector2.Zero, activeMenuDisplay.Message.Font, Color.GhostWhite, parent.SpriteBatch)));

            parent.ChildButtons.ForEach(bttn => bttn.Color = Color.Navy);
        }

        public void StartMenuOpening(GameTime gameTime)
        {
            activeMenuDisplay.Focused = true;

            activeMenuDisplay.ChildButtons.ForEach(bttn => bttn.Point = activeMenuDisplay.Point);

            activeMenuDisplay.ChildButtons[1].ChildButtons.ForEach(bttn => bttn.Y = activeMenuDisplay.ChildButtons[1].Y + 1);

            menuTimer.Start(gameTime, Var.MENU_ACTION_TIME);
        }

        public void EnemyGotCake()
        {
            cake.CurrentHealth--;
        }

        public void EnemyDied(int cashReward)
        {
            money += cashReward;
        }
        #endregion Methods

        #region Draw
        public void Draw()
        {
            moneyDisplay.Draw();
            cakeDisplay.Draw();

            if (activeMenuDisplay.Focused)
            {
                for (int i = activeMenuDisplay.ChildButtons.Count - 1; i >= 0; i--)
                {
                    activeMenuDisplay.ChildButtons[i].Draw();
                    if (activeMenuDisplay.ChildButtons[i].ChildButtons != null)
                        activeMenuDisplay.ChildButtons[i].ChildButtons.ForEach(button => button.Draw());
                }
            }

            activeMenuDisplay.Draw();
        }
        #endregion Draw
    }
}

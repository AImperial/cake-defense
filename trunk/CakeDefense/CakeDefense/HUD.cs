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
        private int money, score, health;
        private Button moneyDisplay, cakeDisplay, activeMenuDisplay;
        private List<Button> activeMenuDisplayButtons;
        private Timer menuTimer;
        #endregion Attributes

        #region Constructor
        public HUD(SpriteBatch sprite, int money, int health, Texture2D infoBoxTex, SpriteFont font)
        {
            this.spriteBatch = sprite;
            this.money = money;
            this.health = health;

            moneyDisplay = new Button(infoBoxTex, new Vector2(2, 2), 120, 40, 2, Color.DarkGray, sprite, new TextObject("$" + money, Vector2.Zero, font, Color.Black, sprite));
            moneyDisplay.Color = Color.DarkKhaki;
            cakeDisplay = new Button(infoBoxTex, new Vector2(125, 2), 120, 40, 2, Color.DarkGray, sprite, new TextObject("Cake: " + health, Vector2.Zero, font, Color.Black, sprite));
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
            get { return health; }
            set { health = value; }
        }

        public Button MenuButton
        {
            get { return activeMenuDisplay; }
        }

        public List<Button> MenuButtonList
        {
            get { return activeMenuDisplayButtons; }
        }
        #endregion Properties

        #region Methods

        public void Update(GameTime gameTime, int cakeHealth)
        {
            moneyDisplay.Message.Message = "$" + money;
            cakeDisplay.Message.Message = "Cake: " + (health = cakeHealth);

            menuTimer.Update(gameTime);
            if (menuTimer.Finished == false)
            {
                activeMenuDisplayButtons[0].Y = activeMenuDisplay.Y + (int)((activeMenuDisplay.Height + 2) * menuTimer.Percent);
                activeMenuDisplayButtons[0].CenterText();
                for (int i = 1; i < activeMenuDisplayButtons.Count; i++)
                {
                    activeMenuDisplayButtons[i].Y = activeMenuDisplayButtons[i - 1].Y + (int)((activeMenuDisplayButtons[i - 1].Height + 2) * menuTimer.Percent);
                    activeMenuDisplayButtons[i].CenterText();
                }
            }
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
            activeMenuDisplayButtons = new List<Button>();

            activeMenuDisplayButtons.Add(new Button(activeMenuDisplay.Texture, new Vector2(Var.TOTAL_WIDTH - 120 - 2, 2), 120, 40, 2, Color.LightCyan, activeMenuDisplay.SpriteBatch,
                new TextObject("Pause", Vector2.Zero, activeMenuDisplay.Message.Font, Color.GhostWhite, activeMenuDisplay.SpriteBatch)));
            activeMenuDisplayButtons.Add(new Button(activeMenuDisplay.Texture, new Vector2(Var.TOTAL_WIDTH - 120 - 2, 2), 120, 40, 2, Color.LightCyan, activeMenuDisplay.SpriteBatch,
                new TextObject("Restart", Vector2.Zero, activeMenuDisplay.Message.Font, Color.GhostWhite, activeMenuDisplay.SpriteBatch)));
            activeMenuDisplayButtons.Add(new Button(activeMenuDisplay.Texture, new Vector2(Var.TOTAL_WIDTH - 120 - 2, 2), 120, 40, 2, Color.LightCyan, activeMenuDisplay.SpriteBatch,
                new TextObject("Exit", Vector2.Zero, activeMenuDisplay.Message.Font, Color.GhostWhite, activeMenuDisplay.SpriteBatch)));

            activeMenuDisplayButtons.ForEach(bttn => bttn.Color = Color.Navy);
        }

        public void StartMenuOpening(GameTime gameTime)
        {
            activeMenuDisplay.Focused = true;

            activeMenuDisplayButtons.ForEach(bttn => bttn.Point = activeMenuDisplay.Point);

            menuTimer.Start(gameTime, Var.MENU_ACTION_TIME);
        }
        #endregion Methods

        #region Draw
        public void Draw()
        {
            moneyDisplay.Draw();
            cakeDisplay.Draw();

            if (activeMenuDisplay.Focused)
            {
                for (int i = activeMenuDisplayButtons.Count - 1; i >= 0; i--)
                    activeMenuDisplayButtons[i].Draw();
            }

            activeMenuDisplay.Draw();
        }
        #endregion Draw
    }
}

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
        private Texture2D stripesTex;
        private int money, score;
        private Cake cake;
        private Button moneyDisplay, cakeDisplay, saveMessage, activeMenuDisplay;
        private List<GameObject> selectionBar;
        private GameTime time;
        private Timer menuTimer, saveTimer;
        private Window costWindow;
        #endregion Attributes

        #region Constructor
        public HUD(SpriteBatch sprite, int money, Texture2D infoBoxTex, SpriteFont font, Cake cake, List<GameObject> towerTrapImages, Texture2D stripes, Game game)
        {
            this.spriteBatch = sprite;
            this.money = money;
            this.cake = cake;
            stripesTex = stripes;

            selectionBar = towerTrapImages;
            int xStart = (Var.GAME_AREA.Width - (selectionBar.Count * 70)) / 2, ndx = 0;
            foreach (GameObject item in selectionBar)
            {
                item.Width = item.Height = 70;
                item.Y = Var.GAME_AREA.Bottom - item.Height;
                item.X = xStart + (ndx++ * item.Width);
                item.Image.Transparency = 100;
            }

            moneyDisplay = new Button(infoBoxTex, new Vector2(2, 2), 120, 40, 2, Color.DarkGray, sprite, new TextObject("$" + money, Vector2.Zero, font, Color.Black, sprite), null);
            moneyDisplay.Color = Color.DarkKhaki;
            cakeDisplay = new Button(infoBoxTex, new Vector2(125, 2), 120, 40, 2, Color.DarkGray, sprite, new TextObject("Cake: " + cake.CurrentHealth, Vector2.Zero, font, Color.Black, sprite), null);
            cakeDisplay.Color = Color.DarkKhaki;

            activeMenuDisplay = new Button(infoBoxTex, new Vector2(Var.TOTAL_WIDTH - 120 - 2, 2), 120, 40, 2, Color.LightCyan, sprite, new TextObject("Menu", Vector2.Zero, font, Color.GhostWhite, sprite), null);
            activeMenuDisplay.Color = Color.Navy;
            saveMessage = new Button(infoBoxTex, new Vector2(Var.GAME_AREA.X + 2, Var.GAME_AREA.Bottom - 40 - 2), 120, 40, 2, Color.MidnightBlue, sprite, new TextObject("Saved!", Vector2.Zero, font, Color.LightCyan, sprite), null);
            saveMessage.Color = Color.Navy;
            PopulateMenuList(game);
            time = game.AnimationTime;
            menuTimer = new Timer(Var.GAME_SPEED);
            saveTimer = new Timer();
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

        public Window CostWindow
        {
            get { return costWindow; }

            set { costWindow = value; }
        }

        public Button MenuButton
        {
            get { return activeMenuDisplay; }
        }

        public List<GameObject> SelectionBar
        {
            get { return selectionBar; }
        }
        #endregion Properties

        #region Methods

        #region Menu / Timer Stuff

        public void Update(GameTime gameTime)
        {
            time = gameTime;
            moneyDisplay.Message.Message = "$" + money; moneyDisplay.CenterText();
            cakeDisplay.Message.Message = "Cake: " + cake.CurrentHealth;

            saveTimer.Update(gameTime, Var.GAME_SPEED);
            if (saveTimer.Finished == false)
            {
                if (saveTimer.Percent > .6)
                {
                    saveMessage.Transparency = (1 - Timer.GetPercentRelative(.6f, saveTimer.Percent, 1f)) * 100;
                }
            }

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
                }
            }
        }

        private void PopulateMenuList(Game game)
        {
            Button parent = activeMenuDisplay;
            parent.ChildButtons = new List<Button>();
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point, parent.Width, parent.Height, 2, Color.LightCyan, parent.SpriteBatch,
                new TextObject("Pause", Vector2.Zero, parent.Message.Font, Color.GhostWhite, parent.SpriteBatch), new ButtonEvent(game.SwitchPauseAndGame)));
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point, parent.Width, parent.Height, 2, Color.LightCyan, parent.SpriteBatch,
                null, null));
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point, parent.Width, parent.Height, 2, Color.LightCyan, parent.SpriteBatch,
                new TextObject("Restart", Vector2.Zero, parent.Message.Font, Color.GhostWhite, parent.SpriteBatch), new ButtonEvent(game.RestartGame)));
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point, parent.Width, parent.Height, 2, Color.LightCyan, parent.SpriteBatch,
                new TextObject("Exit", Vector2.Zero, parent.Message.Font, Color.GhostWhite, parent.SpriteBatch), new ButtonEvent(game.GoToMenu)));

            parent.ChildButtons.ForEach(bttn => bttn.Color = Color.Navy);


            // Adds buttons to button 1's list in menu button's button list (ugg...?)
            parent = activeMenuDisplay.ChildButtons[1];
            int tempWidth = parent.Width / 3;
            parent.ChildButtons = new List<Button>();
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point + Vector2.One, tempWidth - 2, parent.Height - 2, 2, Color.LightCyan, parent.SpriteBatch,
                new TextObject("1x", Vector2.Zero, activeMenuDisplay.Message.Font, Color.Multiply(Color.GhostWhite, .5f), parent.SpriteBatch), new ButtonEvent(game.GameSpeedOne)));
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point + new Vector2(tempWidth, 0) + Vector2.One, tempWidth, parent.Height - 2, 2, Color.LightCyan, parent.SpriteBatch,
                new TextObject("2x", Vector2.Zero, activeMenuDisplay.Message.Font, Color.GhostWhite, parent.SpriteBatch), new ButtonEvent(game.GameSpeedTwo)));
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point + new Vector2(tempWidth * 2, 0) + Vector2.One, tempWidth, parent.Height - 2, 2, Color.LightCyan, parent.SpriteBatch,
                new TextObject("4x", Vector2.Zero, activeMenuDisplay.Message.Font, Color.GhostWhite, parent.SpriteBatch), new ButtonEvent(game.GameSpeedFour)));

            parent.ChildButtons.ForEach(bttn => bttn.Color = Color.Navy);
        }

        public void StartMenuOpening(GameTime gameTime)
        {
            activeMenuDisplay.Focused = true;

            activeMenuDisplay.ChildButtons.ForEach(bttn => bttn.Point = activeMenuDisplay.Point);

            activeMenuDisplay.ChildButtons[1].ChildButtons.ForEach(bttn => bttn.Y = activeMenuDisplay.ChildButtons[1].Y + 1);

            menuTimer.Start(gameTime, Var.MENU_ACTION_TIME);
        }

        #endregion Menu / Timer Stuff

        /// <summary> Checks to see if you can spend a certain ammount of money (Does NOT subtract it). </summary>
        public bool CanSpendMoney(int spending)
        {
            if (money - spending >= 0)
                return true;
            return false;
        }

        public void GameSaved()
        {
            saveTimer.Start(time, Var.SAVE_MESSAGE_TIME);
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

            DrawDropDownMenu();

            if (saveTimer.Finished == false)
                saveMessage.Draw();

            for (int i = 0; i < selectionBar.Count; i++)
            {
                Color clr = Color.Blue;
                if (selectionBar[i] is Trap)
                    clr = Color.Green;

                spriteBatch.Draw(stripesTex, selectionBar[i].Rectangle, clr);

                selectionBar[i].Draw();
            }

            costWindow.Draw();
        }

        public void DrawDropDownMenu()
        {
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

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
        private SpriteFont spriteFont;
        private Texture2D acidTex, fireTex, flyTex, radTex, zapTex, zoltTex;
        private int money, score, maxWave, currentWave;
        private Cake cake;
        private Button moneyDisplay, cakeDisplay, waveDisplay, saveMessage, activeMenuDisplay;
        private List<GameObject> selectionBar;
        private GameTime time;
        private Timer menuTimer, saveTimer;
        private Window costWindow, infoWindow;
        #endregion Attributes

        #region Constructor
        public HUD(SpriteBatch sprite, int money, Texture2D infoBoxTex, SpriteFont font, Cake cake, List<GameObject> towerTrapImages, Texture2D acid, Texture2D fire, Texture2D fly, Texture2D rad, Texture2D zap, Texture2D zolt, Game game)
        {
            this.spriteBatch = sprite;
            this.spriteFont = font;
            this.money = money;
            this.cake = cake;
            acidTex = acid;
            fireTex = fire;
            flyTex = fly;
            radTex = rad;
            zapTex = zap;
            zoltTex = zolt;

            selectionBar = towerTrapImages;
            int xStart = (Var.GAME_AREA.Width - (selectionBar.Count * 70)) / 2, ndx = 0;
            foreach (GameObject item in selectionBar)
            {
                item.Width = item.Height = 70;
                item.Y = Var.GAME_AREA.Bottom - item.Height;
                item.X = xStart + (ndx++ * item.Width);
                item.Image.Transparency = 100;
            }

            moneyDisplay = new Button(infoBoxTex, new Vector2(250, 2), 120, 40, 2, Color.DarkGray, sprite, new TextObject("$" + money, Vector2.Zero, font, Color.Red, sprite), null);
            cakeDisplay = new Button(infoBoxTex, new Vector2(464, 2), 120, 40, 2, Color.DarkGray, sprite, new TextObject("Cake: " + cake.CurrentHealth, Vector2.Zero, font, Color.Red, sprite), null);
            waveDisplay = new Button(infoBoxTex, new Vector2(685, 2), 140, 40, 2, Color.DarkGray, sprite, new TextObject("", Vector2.Zero, font, Color.Red, sprite), null);

            activeMenuDisplay = new Button(infoBoxTex, new Vector2(Var.TOTAL_WIDTH - 370, 2), 120, 40, 2, Color.LightCyan, sprite, new TextObject(" ", Vector2.Zero, font, Color.Red, sprite), null);
            saveMessage = new Button(infoBoxTex, new Vector2(Var.GAME_AREA.X + 2, Var.GAME_AREA.Bottom - 42), 120, 40, 2, Color.MidnightBlue, sprite, new TextObject("Saved!", Vector2.Zero, font, Color.Red, sprite), null);

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

        public int MaxWave
        {
            get { return maxWave; }
            set { maxWave = value;}
        }

        public int CurrentWave
        {
            get { return currentWave; }
            set { currentWave = value + 1; waveDisplay.Message.Message = "Wave: " + currentWave + "/" + maxWave; waveDisplay.CenterText(); }
        }

        public Window CostWindow
        {
            get { return costWindow; }
            set { costWindow = value; }
        }

        public Window InfoWindow
        {
            get { return infoWindow; }
            set { infoWindow = value; }
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
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point, parent.Width, parent.Height, 2, Color.Red, parent.SpriteBatch,
                new TextObject("Pause", Vector2.Zero, parent.Message.Font, Color.Red, parent.SpriteBatch), new ButtonEvent(game.SwitchPauseAndGame)));
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point, parent.Width, parent.Height, 2, Color.Red, parent.SpriteBatch,
                null, null));
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point, parent.Width, parent.Height, 2, Color.Red, parent.SpriteBatch,
                new TextObject("Mute", Vector2.Zero, parent.Message.Font, Color.Red, parent.SpriteBatch), new ButtonEvent(game.Mute_UnMute)));
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point, parent.Width, parent.Height, 2, Color.Red, parent.SpriteBatch,
                new TextObject("Restart", Vector2.Zero, parent.Message.Font, Color.Red, parent.SpriteBatch), new ButtonEvent(game.RestartGame)));
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point, parent.Width, parent.Height, 2, Color.Red, parent.SpriteBatch,
                new TextObject("Exit", Vector2.Zero, parent.Message.Font, Color.Red, parent.SpriteBatch), new ButtonEvent(game.GoToMenu)));

            // Adds buttons to button 1's list in menu button's button list
            parent = activeMenuDisplay.ChildButtons[1];
            int tempWidth = parent.Width / 3;
            parent.ChildButtons = new List<Button>();
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point + Vector2.One, tempWidth - 2, parent.Height - 2, 2, Color.Red, parent.SpriteBatch,
                new TextObject("1x", Vector2.Zero, activeMenuDisplay.Message.Font, Color.Multiply(Color.Maroon, .5f), parent.SpriteBatch), new ButtonEvent(game.GameSpeedOne)));
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point + new Vector2(tempWidth, 0) + Vector2.One, tempWidth, parent.Height - 2, 2, Color.Red, parent.SpriteBatch,
                new TextObject("2x", Vector2.Zero, activeMenuDisplay.Message.Font, Color.Red, parent.SpriteBatch), new ButtonEvent(game.GameSpeedTwo)));
            parent.ChildButtons.Add(new Button(parent.Texture, parent.Point + new Vector2(tempWidth * 2, 0) + Vector2.One, tempWidth, parent.Height - 2, 2, Color.Red, parent.SpriteBatch,
                new TextObject("4x", Vector2.Zero, activeMenuDisplay.Message.Font, Color.Red, parent.SpriteBatch), new ButtonEvent(game.GameSpeedFour)));
        }

        public void StartMenuOpening(GameTime gameTime)
        {
            activeMenuDisplay.Focused = true;
            activeMenuDisplay.ChildButtons.ForEach(bttn => bttn.Point = activeMenuDisplay.Point);
            activeMenuDisplay.ChildButtons[1].ChildButtons.ForEach(bttn => bttn.Y = activeMenuDisplay.ChildButtons[1].Y + 1);
            menuTimer.Start(gameTime, Var.MENU_ACTION_TIME);
        }

        #endregion Menu / Timer Stuff

        /// <summary> Checks to see if you can spend a certain amount of money (Does NOT subtract it). </summary>
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
            waveDisplay.Draw();

            DrawDropDownMenu();

            if (saveTimer.Finished == false)
                saveMessage.Draw();

            for (int i = 0; i < selectionBar.Count; i++)
            {
                if (i == 0)
                    spriteBatch.Draw(radTex, selectionBar[i].Rectangle, Color.White);
                if (i == 1)
                    spriteBatch.Draw(zoltTex, selectionBar[i].Rectangle, Color.White);
                if (i == 2)
                    spriteBatch.Draw(fireTex, selectionBar[i].Rectangle, Color.White);
                if (i == 3)
                    spriteBatch.Draw(acidTex, selectionBar[i].Rectangle, Color.White);
                if (i == 4)
                    spriteBatch.Draw(flyTex, selectionBar[i].Rectangle, Color.White);
                if (i == 5)
                    spriteBatch.Draw(zapTex, selectionBar[i].Rectangle, Color.White);

                //selectionBar[i].Draw();
            }
            costWindow.Draw();

            infoWindow.Draw();
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

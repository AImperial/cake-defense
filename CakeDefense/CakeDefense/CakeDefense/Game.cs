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
    public class Game : Microsoft.Xna.Framework.Game
    {
        #region Attributes
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont largeFont, mediumFont, normalFont, smallFont;
        Texture2D blankTex, cursorTex, mainMenu, instructions, credits;

        public enum GameState { Menu, Instructions, Credits, Game, GameOver }
        public enum ButtonType { Menu }
        private GameState gameState;
        KeyboardState kbState, previouskbState;
        MouseState mouseState, previousMouseState;

        Rectangle mouseLoc;
        Dictionary<ButtonType, Rectangle[]> buttons;
        bool singlePress, musicOn, soundEffectsOn;

        Map map;
        #endregion Attributes

        #region Initialize
        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferHeight = Var.TOTAL_HEIGHT;
            graphics.PreferredBackBufferWidth = Var.TOTAL_WIDTH;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            gameState = GameState.Menu;

            singlePress = false; musicOn = true; soundEffectsOn = true;

            buttons = new Dictionary<ButtonType, Rectangle[]>{
                { ButtonType.Menu, new Rectangle[]{
                    new Rectangle(118, 184, 498, 284),
                    new Rectangle(762, 80, 420, 145),
                    new Rectangle(776, 387, 417, 136) } }
            };

            map = new Map();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            blankTex = this.Content.Load<Texture2D>("Blank");
            cursorTex = this.Content.Load<Texture2D>("Cursor");

            #region Menu
            mainMenu = this.Content.Load<Texture2D>("Menu/MainMenu");
            instructions = this.Content.Load<Texture2D>("Menu/Instructions");
            credits = this.Content.Load<Texture2D>("Menu/Credits");
            #endregion Menu

            #region Spritefonts
            largeFont = this.Content.Load<SpriteFont>("Spritefonts/Large");
            mediumFont = this.Content.Load<SpriteFont>("Spritefonts/Medium");
            normalFont = this.Content.Load<SpriteFont>("Spritefonts/Normal");
            smallFont = this.Content.Load<SpriteFont>("Spritefonts/Small");
            #endregion Spritefonts
        }
        #endregion Initialize

        protected override void Update(GameTime gameTime)
        {
            #region Kb/MouseState
            singlePress = false;

            //Keyboard
            previouskbState = kbState;
            kbState = Keyboard.GetState();

            previousMouseState = mouseState;
            mouseState = Mouse.GetState();

            mouseLoc = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            #endregion Kb/MouseState

            switch (gameState)
            {
                #region GameState.Game
                case GameState.Game:

                    break;
                #endregion GameState.Game

                #region Everything else
                case GameState.Menu:
                    if (CheckIfClicked(buttons[ButtonType.Menu][0]))
                    {
                        gameState = GameState.Game;
                    }
                    else if (CheckIfClicked(buttons[ButtonType.Menu][1]))
                    {
                        gameState = GameState.Instructions;
                    }
                    else if (CheckIfClicked(buttons[ButtonType.Menu][2]))
                    {
                        gameState = GameState.Credits;
                    }
                    break;
                case GameState.Instructions:
                    if (mouseState.RightButton == ButtonState.Pressed)
                        gameState = GameState.Menu;
                    break;
                case GameState.Credits:
                    if (mouseState.RightButton == ButtonState.Pressed)
                        gameState = GameState.Menu;
                    break;
                case GameState.GameOver:

                    break;
                #endregion Everything else
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGreen);
            spriteBatch.Begin();

            switch (gameState)
            {
                #region GameState.Game
                case GameState.Game:
                    map.DrawMap(spriteBatch, blankTex);
                    break;
                #endregion GameState.Game

                #region Everything else
                case GameState.Menu:
                    spriteBatch.Draw(mainMenu, Var.SCREEN_SIZE, Color.White);
                    break;
                case GameState.Instructions:
                    spriteBatch.Draw(instructions, Var.SCREEN_SIZE, Color.White);
                    spriteBatch.DrawString(mediumFont, "Right click to return", new Vector2(15, Var.TOTAL_HEIGHT - mediumFont.MeasureString("-").Y - 10), Color.DarkGreen);
                    break;
                case GameState.Credits:
                    spriteBatch.Draw(credits, Var.SCREEN_SIZE, Color.White);
                    spriteBatch.DrawString(mediumFont, "Right click to return", new Vector2(15, Var.TOTAL_HEIGHT - mediumFont.MeasureString("-").Y - 10), Color.DarkGreen);
                    break;
                case GameState.GameOver:

                    break;
                #endregion Everything else
            }
            spriteBatch.Draw(cursorTex, new Rectangle(mouseLoc.X, mouseLoc.Y, 25, 25), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        #region Mouse / Keyboard Stuff

        #region Single Press / Click
        /// <summary> Processes a singular mouse click </summary>
        /// <returns>If the current button press is the same as the last</returns>
        public bool SingleMouseClick()
        {
            if (mouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed && singlePress == false)
            {
                singlePress = true;
                return true;
            }
            return false;
        }

        /// <summary> Processes a singular key input. </summary>
        /// <param name="key">The Key you wish to check</param>
        /// <returns>If the current key press is the same as the last</returns>
        public bool SingleKeyPress(Keys key)
        {
            if (kbState.IsKeyDown(key) == previouskbState.IsKeyDown(key) || kbState.IsKeyUp(key))
            {
                return false;
            }
            return true;
        }
        #endregion Single Press / Click

        #region CheckIfClicked (Includes SinglePress)
        /// <summary> Checks if your mouse has clicked a specific Rectangle AND if it's a SingleClick(). </summary>
        /// <param name="area">The area being checked</param>
        /// <returns>If clicked</returns>
        public bool CheckIfClicked(Rectangle area)
        {
            bool ValueOn = false;
            if (area.Intersects(mouseLoc))
            {
                if (SingleMouseClick())
                    ValueOn = true;
            }
            return ValueOn;
        }

        /// <summary> Checks if your mouse has clicked a specific Rectangle AND if it's a SingleClick(). </summary>
        /// <param name="area">The object being checked</param>
        /// <returns>If clicked</returns>
        public bool CheckIfClicked(GameObject go)
        {
            return CheckIfClicked(go.Rectangle);
        }
        #endregion CheckIfClickedd (Includes SinglePress)

        #endregion Mouse / Keyboard Stuff

        #region Draw Rectangle
        /// <summary> Draws an outline around a rectangle. </summary>
        /// <param name="rect">The Rectangle To Be Outlined</param>
        private void DrawRectangleOutline(Rectangle rect, Color color)//, int borderSize)
        {
            int borderSize = 1;
            int cB = borderSize / 2 + 1; // Center By

            // Draw the 4 lines as 4 thin boxes:
            // Top, right, bottom, left
            DrawBox(rect.X - cB, rect.Y - cB, rect.Width, borderSize, color);
            DrawBox(rect.X + rect.Width - cB, rect.Y - cB, borderSize, rect.Height + borderSize, color);
            DrawBox(rect.X - cB, rect.Y + rect.Height - cB, rect.Width + borderSize, borderSize, color);
            DrawBox(rect.X - cB, rect.Y - cB, borderSize, rect.Height, color);
        }

        private void DrawBox(int x, int y, int width, int height, Color color)
        {
            // Draw the box
            spriteBatch.Draw(blankTex, new Rectangle(x, y, width, height), color);
        }
        #endregion Draw Rectangle
    }
}

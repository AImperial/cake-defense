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

        public enum GameState { Menu, Instructions, Credits, Game, GameOver }
        private GameState gameState;
        KeyboardState kbState, previouskbState;
        MouseState mouseState, previousMouseState;

        Rectangle mouseLoc;
        bool singlePress, musicOn, soundEffectsOn;
        #endregion Attributes

        #region Initialize
        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }
        #endregion Initialize

        protected override void Update(GameTime gameTime)
        {
            #region Gamepad, Kb/MouseState & Frame Stuff
            singlePress = false;

            //Keyboard
            previouskbState = kbState;
            kbState = Keyboard.GetState();

            previousMouseState = mouseState;
            mouseState = Mouse.GetState();

            mouseLoc = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            #endregion Gamepad, Kb/MouseState & Frame Stuff

            switch (gameState)
            {
                #region GameState.Game
                case GameState.Game:

                    break;
                #endregion GameState.Game

                #region Everything else
                case GameState.Menu:

                    break;
                case GameState.Instructions:

                    break;
                case GameState.Credits:

                    break;
                case GameState.GameOver:

                    break;
                #endregion Everything else
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MediumPurple);

            switch (gameState)
            {
                #region GameState.Game
                case GameState.Game:

                    break;
                #endregion GameState.Game

                #region Everything else
                case GameState.Menu:

                    break;
                case GameState.Instructions:

                    break;
                case GameState.Credits:

                    break;
                case GameState.GameOver:

                    break;
                #endregion Everything else
            }

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
    }
}

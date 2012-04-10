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

        #region Graphic Stuff (Texture2D, SpriteFont, SpriteBatch, GraphicsDeviceManager)
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont largeFont, mediumFont, normalFont, smallFont;
        Texture2D blankTex, cursorTex, mainMenu, instructions, credits, enemyAnimationTest;
        #endregion Graphic Stuff

        #region General Game Stuff (GameState, music stuff, Map, HUD)
        public enum GameState { Menu, Instructions, Credits, Game, GameOver }
        private GameState gameState;
        bool musicOn, soundEffectsOn;
        Map map;
        HUD hud;
        #endregion General Game Stuff

        #region Keyboard / Mouse Stuff (KeyboardState, MouseState, Button Dictionary)
        KeyboardState kbState, previouskbState;
        MouseState mouseState, previousMouseState;
        Rectangle mouseRect;
        Vector2 mousePoint;
        bool singlePress;

        public enum ButtonType { Menu }
        Dictionary<ButtonType, Rectangle[]> buttons;
        #endregion Keyboard / Mouse Stuff

        #region Enemy Stuff (spawn stuff, enemies wave list / active list)
        List<Enemy> enemies;
        List<Queue<Enemy>> waves;
        TimeSpan lastSpawnTime;
        #endregion Enemy Stuff

        #region Tower Stuff (Tower List, heldTower)
        Tower heldTower;
        List<Tower> towers;
        #endregion Tower Stuff

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
            lastSpawnTime = TimeSpan.Zero;
            enemies = new List<Enemy>();
            waves = new List<Queue<Enemy>>();

            buttons = new Dictionary<ButtonType, Rectangle[]>{
                { ButtonType.Menu, new Rectangle[]{
                    new Rectangle(118, 184, 498, 284),
                    new Rectangle(762, 80, 420, 145),
                    new Rectangle(776, 387, 417, 136) } }
            };

            base.Initialize();
        }

        /// <summary>
        /// Objects that use things such as Textures / a SpriteBatch
        /// that need to be made after LoadContent go here.
        /// </summary>
        protected void InitializeAfterLoadContent()
        {
            hud = new HUD(spriteBatch, 0);
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

            #region Sprites
            enemyAnimationTest = this.Content.Load<Texture2D>("Sprites/SpiderSprite");
            #endregion Sprites

            InitializeAfterLoadContent();
        }
        #endregion Initialize

        #region Update
        protected override void Update(GameTime gameTime)
        {
            #region Kb/MouseState
            singlePress = false;

            //Keyboard
            previouskbState = kbState;
            kbState = Keyboard.GetState();

            previousMouseState = mouseState;
            mouseState = Mouse.GetState();

            mouseRect = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            mousePoint = new Vector2(mouseState.X, mouseState.Y);
            #endregion Kb/MouseState

            switch (gameState)
            {
                #region GameState.Game
                case GameState.Game:

                    QuickKeys();

                    #region Wave Stuff
                    if (waves.Count > 0 && waves[0].Count > 0 && (lastSpawnTime + Var.TIME_BETWEEN_SPAWNS).TotalMilliseconds < gameTime.TotalGameTime.TotalMilliseconds)
                    {
                        enemies.Add(waves[0].Dequeue());
                        enemies[enemies.Count - 1].Start(gameTime);
                        lastSpawnTime = new TimeSpan(0, 0, 0, 0, (int)gameTime.TotalGameTime.TotalMilliseconds);
                    }
                    else if (waves.Count > 0 && waves[0].Count == 0 &&  (lastSpawnTime + Var.TIME_BETWEEN_WAVES).TotalMilliseconds < gameTime.TotalGameTime.TotalMilliseconds)
                    {
                        waves.RemoveAt(0);
                    }else if (waves.Count == 0){
                        gameState = GameState.GameOver;
                    }
                    #endregion Wave Stuff

                    enemies.ForEach(enemy => enemy.Move(gameTime));

                    if (heldTower != null)
                        heldTower.Point = mousePoint;

                    #region If Mouse Clicked
                    if (CheckIfClicked(Var.GAME_AREA))
                    {
                        singlePress = false;
                        foreach (Tower tower in towers)
                        {
                            if (CheckIfClicked(tower.Rectangle))
                            {
                                // Info
                                break;
                            }
                        }

                        foreach (Enemy enemy in enemies)
                        {
                            if (enemy.IsActive && CheckIfClicked(enemy.Rectangle))
                            {
                                // Info
                                break;
                            }
                        }

                        foreach (Tile tile in map.Tiles)
                        {
                            if (CheckIfClicked(tile.Rectangle))
                            {
                                if(tile is Tile_Tower)
                                {
                                    if (heldTower != null && tile.OccupiedBy == null && hud.CanSpendMoney(heldTower.Cost)) 
                                    {
                                        hud.Money -= heldTower.Cost;
                                        heldTower.Place((Tile_Tower)tile);
                                        towers.Add(heldTower);
                                        heldTower = null;
                                    }
                                }
                                else if(tile is Tile_Path)
                                {

                                }
                                break;
                            }
                        }
                    }
                    #endregion If Mouse Clicked

                    break;
                #endregion GameState.Game

                #region Everything else
                case GameState.Menu:
                    if (CheckIfClicked(buttons[ButtonType.Menu][0]))
                    {
                        NewGame();
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
                    if (SingleMouseClick() || SingleKeyPress(Keys.Space))
                        gameState = GameState.Menu;
                    break;
                #endregion Everything else
            }

            base.Update(gameTime);
        }
        #endregion Update

        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGreen);
            spriteBatch.Begin();

            switch (gameState)
            {
                #region GameState.Game
                case GameState.Game:

                    map.DrawMap(blankTex, smallFont);

                    foreach (Enemy enemy in enemies)
                        enemy.Draw(gameTime);

                    foreach (Tower tower in towers)
                        tower.Draw();
                    if (heldTower != null)
                        heldTower.Draw();

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
                    spriteBatch.DrawString(mediumFont, "Click or Press Space to continue", new Vector2(15, Var.TOTAL_HEIGHT - mediumFont.MeasureString("-").Y - 10), Color.Blue);
                    break;
                #endregion Everything else
            }
            spriteBatch.Draw(cursorTex, new Rectangle(mouseRect.X, mouseRect.Y, 25, 25), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion Draw

        #region Mouse / Keyboard Stuff

        #region Quick Keys
        public void QuickKeys()
        {
            if (SingleKeyPress(Keys.D1)) 
            {
                heldTower = NewTower(Var.TowerType.Basic);
            }
        }
        #endregion Quick Keys

        #region Single Press / Click
        /// <summary> Processes a singular mouse click </summary>
        /// <returns>If the current button press is the same as the last</returns>
        public bool SingleMouseClick()
        {
            if (singlePress == false && mouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
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
            if (kbState.IsKeyUp(key) && previouskbState.IsKeyDown(key))
            {
                return true;
            }
            return false;
        }
        #endregion Single Press / Click

        #region CheckIfClicked (Includes SinglePress)
        /// <summary> Checks if your mouse has clicked a specific Rectangle AND if it's a SingleClick(). </summary>
        /// <param name="area">The area being checked</param>
        /// <returns>If clicked</returns>
        public bool CheckIfClicked(Rectangle area)
        {
            if (area.Intersects(mouseRect))
            {
                if (SingleMouseClick())
                    return true;
            }
            return false;
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

        #region New Game / LoadGame / SaveGame
        public void NewGame()
        {
            map = new Map(32, 18, spriteBatch);
            Path path0 = map.FindPath(map.Tiles[0, 11], map.Tiles[23, 17], 1);

            Queue<Enemy> wave1 = new Queue<Enemy>();
            wave1.Enqueue(NewEnemy(Var.EnemyType.Spider, path0, 2));
            wave1.Enqueue(NewEnemy(Var.EnemyType.Spider, path0, 2));
            wave1.Enqueue(NewEnemy(Var.EnemyType.Spider, path0, 2));
            wave1.Enqueue(NewEnemy(Var.EnemyType.Spider, path0, 2));
            waves.Add(wave1);

            Queue<Enemy> wave2 = new Queue<Enemy>();
            wave2.Enqueue(NewEnemy(Var.EnemyType.Spider, path0, 5));
            wave2.Enqueue(NewEnemy(Var.EnemyType.Spider, path0, 5));
            wave2.Enqueue(NewEnemy(Var.EnemyType.Spider, path0, 5));
            waves.Add(wave2);

            towers = new List<Tower>();
            towers.Add(NewTower(Var.TowerType.Basic));
            towers.ForEach(t => t.Place((Tile_Tower)map.Tiles[16, 2]));
        }
        #endregion New Game / LoadGame / SaveGame

        #region New Enemy / Tower
        private Enemy NewEnemy(Var.EnemyType type, Path path, int speed)
        {
            #region Spider
            if (type == Var.EnemyType.Spider)
            {
                ImageObject image = new ImageObject(
                        enemyAnimationTest,
                        0,
                        0,
                        Var.ENEMY_SIZE,
                        Var.ENEMY_SIZE,
                        3,
                        0,
                        0,
                        16,
                        16,
                        0,
                        0,
                        Color.White,
                        ImageObject.RIGHT,
                        Vector2.Zero,
                        SpriteEffects.None,
                        spriteBatch);
                image.CenterOrigin();

                return new Enemy(
                    image,
                    Var.MAX_ENEMY_HEALTH,
                    2,
                    speed,
                    path);
            }
            #endregion Spider

            return null;
        }

        private Tower NewTower(Var.TowerType type)
        {
            #region Basic
            if (type == Var.TowerType.Basic)
            {
                return new Tower(Var.MAX_TOWER_HEALTH, 100, 2, 2, Var.TILE_SIZE, Var.TILE_SIZE, spriteBatch, Color.Blue, blankTex);
            }
            #endregion Basic

            return null;
        }
        #endregion New Enemy / Tower
    }
}

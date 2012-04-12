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
        Texture2D blankTex, cursorTex, mainMenu, instructions, credits, gameOver, bulletTex, enemyAnimationTest, pixel, towerTex;
        GameTime animationTotalTime;
        TimeSpan pausedTime;
        #endregion Graphic Stuff

        #region General Game Stuff (GameState, music stuff, Map, HUD)
        public enum GameState { Menu, Instructions, Credits, Game, Paused, GameOver }
        private GameState gameState;
        bool debugOn, musicOn, soundEffectsOn;
        float difficulty;
        int level, wave;
        Map map;
        Cake cake;
        HUD hud;

        Button turnDebugOn;
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
        List<Path> paths;
        List<Enemy> enemies;
        List<Queue<Enemy>> waves;
        Timer spawnTimer;
        #endregion Enemy Stuff

        #region Tower/Trap Stuff (Tower List, heldItem)
        GameObject heldItem;
        List<Tower> towers;
        List<Trap> traps;
        #endregion Tower/Trap Stuff

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
            pixel = new Texture2D(GraphicsDevice, 1, 1); pixel.SetData<Color>(new Color[] { Color.White });
            gameState = GameState.Menu;

            singlePress = false; debugOn = false; musicOn = true; soundEffectsOn = true;

            buttons = new Dictionary<ButtonType, Rectangle[]>{
                { ButtonType.Menu,
                    new Rectangle[] {
                        new Rectangle(109, 124, 498, 284),
                        new Rectangle(762, 80, 420, 145),
                        new Rectangle(776, 387, 417, 136),
                        new Rectangle(97, 459, 607, 195),
                        new Rectangle(0, 0, 163, 79)
                    }
                }
            };

            base.Initialize();
        }

        /// <summary>
        /// Objects that use things such as Textures / a SpriteBatch
        /// that need to be made after LoadContent go here.
        /// </summary>
        protected void InitializeAfterLoadContent()
        {
            // Most of these are in NewGame()
            turnDebugOn = new Button(blankTex, new Vector2(25), 300, 300, 1, Color.DarkOrange, spriteBatch, new TextObject("", Vector2.Zero, mediumFont, Color.GhostWhite, spriteBatch));
            turnDebugOn.Color = Color.Black;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Var.BLANK_TEX = blankTex = this.Content.Load<Texture2D>("Blank");
            cursorTex = this.Content.Load<Texture2D>("Cursor");

            #region Menu
            mainMenu = this.Content.Load<Texture2D>("Menu/MainMenu");
            instructions = this.Content.Load<Texture2D>("Menu/Instructions");
            credits = this.Content.Load<Texture2D>("Menu/Credits");
            gameOver = this.Content.Load<Texture2D>("Menu/GameOver");
            #endregion Menu

            #region Spritefonts
            largeFont = this.Content.Load<SpriteFont>("Spritefonts/Large");
            mediumFont = this.Content.Load<SpriteFont>("Spritefonts/Medium");
            normalFont = this.Content.Load<SpriteFont>("Spritefonts/Normal");
            smallFont = this.Content.Load<SpriteFont>("Spritefonts/Small");
            #endregion Spritefonts

            #region Sprites
            enemyAnimationTest = this.Content.Load<Texture2D>("Sprites/SpiderSprite");
            bulletTex = this.Content.Load<Texture2D>("Sprites/Bullet");
            towerTex = this.Content.Load<Texture2D>("Sprites/can");
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
            animationTotalTime = new GameTime(gameTime.TotalGameTime - pausedTime, TimeSpan.Zero);

            switch (gameState)
            {
                #region GameState.Game
                case GameState.Game:

                    QuickKeys();
                    spawnTimer.Update(animationTotalTime);

                    #region Wave Stuff
                    if (waves.Count > 0 && waves[0].Count > 0 && spawnTimer.Finished)
                    {
                        enemies.Add(waves[0].Dequeue());
                        enemies[enemies.Count - 1].Start(animationTotalTime);

                        if (waves[0].Count > 0)
                            spawnTimer.Start(animationTotalTime, Var.TIME_BETWEEN_SPAWNS);
                        else
                            spawnTimer.Start(animationTotalTime, Var.TIME_BETWEEN_WAVES);

                        if (waves.Count == 1 && waves[0].Count == 0)
                        {
                            waves.RemoveAt(0);
                            wave = 0;
                            SaveGame();//save
                            Console.WriteLine("saved");
                        }
                    }
                    else if (waves.Count > 0 && waves[0].Count == 0 && spawnTimer.Finished)
                    {
                        waves.RemoveAt(0);
                        wave++;
                        SaveGame();//save
                        Console.WriteLine("saved");
                    }
                    else if (waves.Count == 0 && enemies.Count == 0)
                    {
                        NextLevel();
                    }
                    #endregion Wave Stuff
                    
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].Update(animationTotalTime, traps);

                        if (enemies[i].IsActive == false)
                        {
                            enemies.RemoveAt(i);
                            i--;
                        }
                    }

                    towers.ForEach(tower => tower.Fire(enemies));
                    traps.ForEach(trap => traps.Remove(trap.RemoveIfCan()));

                    if (heldItem != null)
                        heldItem.Point = mousePoint;

                    #region Collision
                    foreach(Tower tower in towers)
                    {
                        foreach(Enemy enemy in enemies)
                            tower.CheckCollision(enemy);
                    }
                    #endregion Collision

                    hud.Update(animationTotalTime);

                    #region Mouse Over

                    #region Menu Display (note: clicking on it is with other click stuff below)
                    if (hud.MenuButton.Focused == false && mouseRect.Intersects(hud.MenuButton.Rectangle))
                    {
                        hud.StartMenuOpening(animationTotalTime);
                    }
                    else if (hud.MenuButton.Focused)
                    {
                        // This rectangle is the rectangle from top of menu bttn to bottom of last menu option.
                        if (mouseRect.Intersects(new Rectangle(hud.MenuButton.X, hud.MenuButton.Y, hud.MenuButton.Width, (hud.MenuButton.ChildButtons.Last().Y + hud.MenuButton.ChildButtons.Last().Height) - hud.MenuButton.Y)))
                            hud.MenuButton.Focused = true;
                        else
                            hud.MenuButton.Focused = false;
                    }
                    #endregion Menu Display

                    #endregion Mouse Over

                    #region If Mouse Clicked
                    if (CheckIfClicked(Var.GAME_AREA))
                    {
                        singlePress = false;

                        #region Click Something (normally)
                        if (heldItem == null)
                        {
                            #region Menu List Buttons
                            if (hud.MenuButton.Focused)
                            {
                                for (int i = 0; i < hud.MenuButton.ChildButtons.Count; i++)
                                {
                                    if (CheckIfClicked(hud.MenuButton.ChildButtons[i].Rectangle))
                                    {
                                        switch (i)
                                        {
                                            case 0://Pause
                                                gameState = GameState.Paused;
                                                break;
                                            case 1://Change Game Speed
                                                #region Change Speed
                                                singlePress = false;
                                                for (int j = 0; j < hud.MenuButton.ChildButtons[i].ChildButtons.Count; j++)
                                                {
                                                    if (CheckIfClicked(hud.MenuButton.ChildButtons[i].ChildButtons[j].Rectangle))
                                                    {
                                                        switch (j)
                                                        {
                                                            case 0:
                                                                Var.GAME_SPEED = 1;
                                                                break;
                                                            case 1:
                                                                Var.GAME_SPEED = 2;
                                                                break;
                                                            case 2:
                                                                Var.GAME_SPEED = 4;
                                                                break;
                                                        }

                                                    }

                                                }
                                                break;
                                                #endregion Change Speed
                                            case 2://Restart
                                                NewGame();
                                                break;
                                            case 3://Exit
                                                gameState = GameState.Menu;
                                                break;
                                        }
                                    }
                                }
                            }
                            #endregion Menu List Buttons

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
                        }
                        #endregion Click Something (normally)

                        #region Place Something
                        else
                        {
                            foreach (Tile tile in map.Tiles)
                            {
                                if (CheckIfClicked(tile.Rectangle))
                                {
                                    if (tile is Tile_Tower)
                                    {
                                        if (heldItem != null && heldItem is Tower && tile.OccupiedBy == null && hud.CanSpendMoney(((Tower)heldItem).Cost))
                                        {
                                            hud.Money -= ((Tower)heldItem).Cost;
                                            ((Tower)heldItem).Place((Tile_Tower)tile);
                                            towers.Add(((Tower)heldItem));
                                            heldItem = null;
                                        }
                                    }
                                    else if (tile is Tile_Path)
                                    {
                                        if (heldItem != null && heldItem is Trap && tile.OccupiedBy == null && hud.CanSpendMoney(((Trap)heldItem).Cost))
                                        {
                                            hud.Money -= ((Trap)heldItem).Cost;
                                            ((Trap)heldItem).Place((Tile_Path)tile);
                                            traps.Add(((Trap)heldItem));
                                            heldItem = null;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion Place Something
                    }
                    else if (mouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed)
                    {
                        heldItem = null;
                    }
                    #endregion If Mouse Clicked

                    if (hud.Health <= 0)
                        gameState = GameState.GameOver;

                    break;
                #endregion GameState.Game

                #region Everything else
                case GameState.Paused:
                    pausedTime += gameTime.ElapsedGameTime;
                    if (SingleKeyPress(Keys.P))
                    {
                        gameState = GameState.Game;
                    }
                    else if (CheckIfClicked(hud.MenuButton.ChildButtons[0].Rectangle))
                    {
                        gameState = GameState.Game;
                    }
                    else if (CheckIfClicked(turnDebugOn.Rectangle))
                    {
                        debugOn = !debugOn;
                    }
                    break;
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
                    else if (CheckIfClicked(buttons[ButtonType.Menu][3]) && CheckForSave())
                    {
                        ContinueGame();
                        gameState = GameState.Game;
                    }
                    else if (CheckIfClicked(buttons[ButtonType.Menu][4]) || SingleKeyPress(Keys.Escape))
                    {
                        Environment.Exit(0);
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
                case GameState.Game: case GameState.Paused:

                    map.DrawMap(blankTex, smallFont);

                    cake.Draw();
                    towers.ForEach(tower => tower.Draw());
                    traps.ForEach(trap => trap.Draw(gameTime));
                    enemies.ForEach(enemy => enemy.Draw(gameTime));

                    if (heldItem != null)
                        heldItem.Draw();

                    #region Debug Stuff
                    if (debugOn)
                    {
                        towers.ForEach(tower => DrawCircle(tower.Center, tower.FireRadius, 25, Color.Red));
                        enemies.ForEach(enemy => enemy.DrawRectangleOutline(1, Color.DarkCyan, blankTex));
                    }
                    #endregion Debug Stuff

                    hud.Draw();

                    // break; is in Pause.

                #endregion GameState.Game

                #region Everything else

                #region GameState.Paused
                // Draws everything in GameState.Game (from above) plus this
                if (gameState == GameState.Paused)
                {
                    spriteBatch.Draw(blankTex, Var.SCREEN_SIZE, Var.PAUSE_GRAY);
                    hud.MenuButton.ChildButtons[0].Draw();
                    spriteBatch.DrawString(largeFont, "Press [P] or click \'Pause\'", new Vector2(Var.GAME_AREA.X + (Var.GAME_AREA.Width - largeFont.MeasureString("Press [P] or click \'Pause\'").X) / 2, Var.TOTAL_HEIGHT - largeFont.MeasureString("-").Y - 10), Color.YellowGreen);

                    #region Debug
                    if (debugOn)
                    {
                        if (mouseRect.Intersects(turnDebugOn.Rectangle))
                        {
                            turnDebugOn.Message.Message = "Fine, turn me off.\n\"You can't\nHANDLE\nthe TRUTH!\"";
                            turnDebugOn.CenterText();
                            turnDebugOn.Draw();
                        }
                        else
                        {
                            turnDebugOn.Message.Message = "Hi.";
                            turnDebugOn.CenterText();
                            turnDebugOn.Draw();
                        }
                    }
                    else
                    {
                        if(mouseRect.Intersects(turnDebugOn.Rectangle))
                        {
                            turnDebugOn.Message.Message = "Turn on Debugging.\nYou know you wanna.";
                            turnDebugOn.CenterText();
                            turnDebugOn.Draw();
                        }
                    }
                    #endregion Debug
                }
                break;

                #endregion GameState.Paused

                case GameState.Menu:
                    spriteBatch.Draw(mainMenu, Var.SCREEN_SIZE, Color.White);
                    if (CheckForSave() == false)
                        spriteBatch.Draw(blankTex, buttons[ButtonType.Menu][3], Color.Black);
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
                    spriteBatch.Draw(gameOver, Var.SCREEN_SIZE, Color.White);
                    spriteBatch.DrawString(mediumFont, "Click or Press Space to continue", new Vector2(15, Var.TOTAL_HEIGHT - mediumFont.MeasureString("-").Y - 10), Color.DarkGreen);
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
        /// <summary> Called in GameState.Game </summary>
        public void QuickKeys()
        {
            if (SingleKeyPress(Keys.D1))
            {
                heldItem = NewTower(Var.TowerType.Basic);
            }
            if (SingleKeyPress(Keys.D0))
            {
                heldItem = NewTrap(Var.TrapType.Basic);
            }
            if (SingleKeyPress(Keys.M))
            {
                musicOn = soundEffectsOn = !musicOn;
            }
            if (SingleKeyPress(Keys.P))
            {
                gameState = GameState.Paused;
            }
            if (SingleKeyPress(Keys.R))
            {
                NewGame();
            }
            if (SingleKeyPress(Keys.Escape))
            {
                gameState = GameState.Menu;
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

        #region NextLevel / New Game / LoadGame / SaveGame

        #region Next Level
        public void NextLevel()
        {
            level++;
            wave = 0;
            LoadWaves(level, wave);

            if (waves.Count == 0)
            {
                gameState = GameState.GameOver;
                DeleteSave();
            }
        }
        #endregion Next Level

        #region New / Continued Game
        private void SetUpGame()
        {
            Var.GAME_SPEED = 1;
            difficulty = 1f;
            pausedTime = TimeSpan.Zero;
            animationTotalTime = new GameTime(TimeSpan.Zero, TimeSpan.Zero);
            enemies = new List<Enemy>();
            towers = new List<Tower>();
            traps = new List<Trap>();
            waves = new List<Queue<Enemy>>();
            spawnTimer = new Timer(Var.GAME_SPEED);
        }

        public void NewGame()
        {
            SetUpGame();

            cake = new Cake(Var.MAX_CAKE_HEALTH, 600, 80, 120, 120, spriteBatch, blankTex);
            hud = new HUD(spriteBatch, Var.START_MONEY, blankTex, mediumFont, cake);

            level = wave = 0;
            map = new Map(level, spriteBatch);

            paths = new List<Path>();
            paths.Add(map.FindPath(map.Tiles[0, 11], map.Tiles[23, 17], 1));

            // Gets enemies
            LoadWaves(level, wave);
        }

        public void ContinueGame()
        {
            SetUpGame();
            LoadGame();

            paths = new List<Path>();
            paths.Add(map.FindPath(map.Tiles[0, 11], map.Tiles[23, 17], 1));

            // Gets enemies
            LoadWaves(level, wave); // level / wave taken from LoadGame();
        }
        #endregion New / Continued Game

        #region Load Waves
        /// <summary> Loads wave data from a .lvl file. </summary>
        /// <param name="level">default = 0</param>
        /// <param name="waveNum">default = 0</param>
        private void LoadWaves(int level, int waveNum)
        {
            int wave = waveNum;
            string[] infoArray = null;
            string firstLine = null;

            while (File.Exists("Game Levels/Level" + level + "/wave" + wave + ".lvl"))
            {
                Queue<Enemy> thisWave = new Queue<Enemy>();
                StreamReader reader = new StreamReader("Game Levels/Level" + level + "/wave" + wave + ".lvl");
                infoArray = null;
                firstLine = null;
                while ((firstLine = reader.ReadLine()) != null)
                {
                    // 0-Type ||| 1-PathType ||| 2-speed ||| 3-health ||| 4-damage
                    if (firstLine.Contains("//"))
                        firstLine.Remove(firstLine.IndexOf("//"));
                    infoArray = firstLine.Split('-');

                    thisWave.Enqueue(
                        NewEnemy(
                        (Var.EnemyType)Enum.Parse(typeof(Var.EnemyType), infoArray[0], true),
                        paths[Int32.Parse(infoArray[1])],
                        float.Parse(infoArray[2]),
                        Int32.Parse(infoArray[3]),
                        Int32.Parse(infoArray[4])
                        )
                    );
                }
                if(thisWave.Count > 0)
                    waves.Add(thisWave);
                wave++;
                difficulty += Var.WAVES_DIFFICULTY_INCREASE;
            }
        }
        #endregion Load Waves

        #region Load / Save Game

        #region Load
        /// <summary> Loads Game info such as money, cake, level, wave, saved towers, and saved traps. also MAP is created here. </summary>
        public void LoadGame()
        {
            if (File.Exists("Save.sav"))
            {
                StreamReader reader = new StreamReader("Save.sav");
                string[] infoArray = null;
                string firstLine = null;
                if ((firstLine = reader.ReadLine()) != null)
                {
                    if (firstLine.Contains("//"))
                        firstLine = firstLine.Remove(firstLine.IndexOf("//"));

                    // 0-Money ||| 1-CakeLeft ||| 2-Level ||| 3-Wave
                    infoArray = firstLine.Split('-');

                    cake = new Cake(Int32.Parse(infoArray[1]), 600, 80, 120, 120, spriteBatch, blankTex);
                    hud = new HUD(spriteBatch, Int32.Parse(infoArray[0]), blankTex, mediumFont, cake);
                    level = Int32.Parse(infoArray[2]);
                    wave = Int32.Parse(infoArray[3]);

                    // Map is declared here as you need the map to place towers and traps, but you need what level to load.
                    map = new Map(level, spriteBatch);

                    string curType = reader.ReadLine();
                    Tower tempTower; string[] coord;
                    while (curType == "--Towers--")
                    {
                        firstLine = reader.ReadLine();
                        if (firstLine == null || (firstLine[0] == '-' && firstLine[1] == '-')) {
                            curType = firstLine;
                            break;
                        }
                        if (firstLine.Contains("//"))
                            firstLine = firstLine.Remove(firstLine.IndexOf("//"));

                        // 0-Type ||| 1-tile (x,y) ||| 2-health ||| 3-AttackType
                        infoArray = firstLine.Split('-'); coord = infoArray[1].Split(',');
                        tempTower = LoadTower((Var.TowerType)Enum.Parse(typeof(Var.TowerType), infoArray[0], true), Int32.Parse(infoArray[2]), (Tower.Attacktype)Enum.Parse(typeof(Tower.Attacktype), infoArray[3], true));
                        tempTower.Place((Tile_Tower)map.Tiles[Int32.Parse(coord[0]), Int32.Parse(coord[1])]);
                        towers.Add(tempTower);
                    }

                    Trap tempTrap;
                    while (curType == "--Traps--")
                    {
                        firstLine = reader.ReadLine();
                        if (firstLine == null || (firstLine[0] == '-' && firstLine[1] == '-'))
                        {
                            curType = firstLine;
                            break;
                        }
                        if (firstLine.Contains("//"))
                            firstLine = firstLine.Remove(firstLine.IndexOf("//"));

                        // 0-Type ||| 1-tile (x,y) ||| 2-health
                        infoArray = firstLine.Split('-'); coord = infoArray[1].Split(',');
                        tempTrap = LoadTrap((Var.TrapType)Enum.Parse(typeof(Var.TrapType), infoArray[0], true), Int32.Parse(infoArray[2]));
                        tempTrap.Place((Tile_Path)map.Tiles[Int32.Parse(coord[0]), Int32.Parse(coord[1])]);
                        traps.Add(tempTrap);
                    }
                }
                reader.Close();
                reader.Dispose();
            }
        }
        #endregion Load

        #region Save
        public void SaveGame()
        {
            StreamWriter writer = new StreamWriter("Save.sav");
            // 0-Money ||| 1-CakeLeft ||| 2-Level ||| 3-Wave
            writer.WriteLine(hud.Money + "-" + hud.Health + "-" + level + "-" + wave + "// 0-Money ||| 1-CakeLeft ||| 2-Level ||| 3-Wave");

            // 0-Type ||| 1-tile (x,y) ||| 2-health ||| 3-AttackType
            writer.WriteLine("--Towers--");
            towers.ForEach(tower => writer.WriteLine(tower.Type + "-" + (tower.OccupiedTile.TileNum.X + "," + tower.OccupiedTile.TileNum.Y) + "-" + tower.CurrentHealth + "-" + tower.AttackType));

            // 0-Type ||| 1-tile (x,y) ||| 2-health
            writer.WriteLine("--Traps--");
            traps.ForEach(trap => writer.WriteLine(trap.Type + "-" + (trap.OccupiedTile.TileNum.X + "," + trap.OccupiedTile.TileNum.Y) + "-" + trap.CurrentHealth));
            writer.Close();
            writer.Dispose();
        }

        public void DeleteSave()
        {
            StreamWriter writer = new StreamWriter("Save.sav");
            writer.Close();
            writer.Dispose();
        }
        #endregion Save

        #region Check If Saved Game
        public bool CheckForSave()
        {
            if (File.Exists("save.sav"))
            {
                StreamReader reader = new StreamReader("save.sav");
                if (reader.ReadLine() != null)
                {
                    reader.Close();
                    reader.Dispose();
                    return true;
                }
                reader.Close();
                reader.Dispose();
            }
            return false;
        }
        #endregion Check If Saved Game

        #endregion Load / Save Game

        #endregion New Game / LoadGame / SaveGame

        #region New Enemy / Tower / Trap

        #region Enemy
        private Enemy NewEnemy(Var.EnemyType type, Path path, float speed, int health, int damage)
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
                    spriteBatch
                );
                image.CenterOrigin();

                return new Enemy(
                    image,
                    (int)(health * difficulty),
                    (int)(damage * difficulty),
                    speed * difficulty,
                    path,
                    blankTex,
                    hud
                );
            }
            #endregion Spider

            return null;
        }
        #endregion Enemy

        #region Tower
        private Tower NewTower(Var.TowerType type)
        {
            #region Basic
            if (type == Var.TowerType.Basic)
            {
                return LoadTower(type, Var.MAX_TOWER_HEALTH, Tower.Attacktype.None);
            }
            #endregion Basic

            return null;
        }

        private Tower LoadTower(Var.TowerType type, int health, Tower.Attacktype attackType)
        {
            #region Basic
            if (type == Var.TowerType.Basic)
            {
                return new Tower(
                    200,
                    health,
                    100,
                    2,
                    500,//Fire Rate in ms
                    Var.BASE_BULLET_SPEED,
                    Var.TILE_SIZE,
                    Var.TILE_SIZE,
                    spriteBatch,
                    towerTex,
                    bulletTex,
                    attackType
                );
            }
            #endregion Basic

            return null;
        }
        #endregion Tower

        #region Trap
        private Trap NewTrap(Var.TrapType type)
        {
            #region Basic
            if (type == Var.TrapType.Basic)
            {
                return LoadTrap(type, 2);
            }
            #endregion Basic

            return null;
        }

        private Trap LoadTrap(Var.TrapType type, int health)
        {
            #region Basic
            if (type == Var.TrapType.Basic)
            {
                ImageObject image = new ImageObject(
                    blankTex,
                    0, 0,
                    Var.TRAP_SIZE, Var.TRAP_SIZE,
                    spriteBatch
                );

                return new Trap(
                    2,
                    5,
                    50,
                    image,
                    blankTex
                );
            }
            #endregion Basic

            return null;
        }
        #endregion Trap

        #endregion New Enemy / Tower / Trap

        #region Draw Circle
        // NOTE: this code taken from MessiahAndrw at http://forums.create.msdn.com/forums/t/7414.aspx (and then modified some =p)
        
        /// <summary>
        /// Creates a circle starting from 0, 0.
        /// </summary>
        /// <param name="radius">The radius (half the width) of the circle.</param>
        /// <param name="sides">The number of sides on the circle (the more the detailed).</param>
        public void DrawCircle(Vector2 cCenter, float radius, int sides, Color color)
        {
            List<Vector2> vectors = new List<Vector2>();

            float max = 2 * (float)Math.PI;
            float step = max / (float)sides;

            for (float theta = 0; theta < max; theta += step)
            {
                vectors.Add(new Vector2(cCenter.X + radius * (float)Math.Cos((double)theta),
                    cCenter.Y + radius * (float)Math.Sin((double)theta)));
            }

            // then add the first vector again so it's a complete loop
            vectors.Add(vectors.First());

            RenderCircle(vectors, color);
        }

        /// <summary>
        /// Renders the primtive line object.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use to render the primitive line object.</param>
        public void RenderCircle(List<Vector2> vectors, Color color)
        {
            if (vectors.Count < 2)
                return;

            Vector2 start, end, scale;
            int lineWidth = 1;
            float rotation;

            for (int i = 1; i <= vectors.Count; i++)
            {
                start = vectors[i - 1];
                if (i == vectors.Count)
                    end = vectors[0];
                else
                    end = vectors[i];

                scale = new Vector2(Vector2.Distance(end, start), lineWidth);

                // Calculate the rotation
                rotation = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

                // Draw
                spriteBatch.Draw(pixel, start, null, color, rotation, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            }
        }
        #endregion Draw Circle
    }
}

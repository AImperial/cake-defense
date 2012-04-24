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
        Texture2D blankTex, stripesTex, cursorTex, bulletTex, ant, spider, beetle, pixel, towerTex, cakeTex, acidTex, slowTex, zapperTex, cakepieceTex, menuBttnTex, infoBoxTex;
        Dictionary<GameState, Texture2D> menuBackgrounds;
        GameTime animationTotalTime; public GameTime AnimationTime { get { return animationTotalTime; } }
        TimeSpan pausedTime;
        #endregion Graphic Stuff

        #region General Game Stuff (GameState, music stuff, Map, HUD)
        public enum GameState { Menu, Instructions, Credits, Game, Paused, GameOver_Lose, GameOver_Win }
        private GameState gameState;
        bool debugOn, musicOn, soundEffectsOn, drawCursor;
        float difficulty;
        int level, wave;
        Map map;
        Cake cake;
        HUD hud;
        #endregion General Game Stuff

        #region Keyboard / Mouse Stuff (KeyboardState, MouseState, Button Dictionary)
        KeyboardState kbState, previouskbState;
        MouseState mouseState, previousMouseState;
        Rectangle mouseRect;
        Vector2 mousePoint;
        bool singlePress;

        Dictionary<GameState, List<Button>> buttons;
        #endregion Keyboard / Mouse Stuff

        #region Enemy Stuff (spawn stuff, enemies wave list / active list)
        List<Path> paths;
        List<Enemy> enemies;
        List<List<Enemy>> prevWaves;
        List<Queue<Enemy>> waves;
        List<Vector2> droppedCake;
        Timer spawnTimer;
        #endregion Enemy Stuff

        #region Tower/Trap Stuff (Tower List, heldItem)
        GameObject heldItem, selectedItem;
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

            base.Initialize();
        }

        /// <summary>
        /// Objects that use things such as Textures / a SpriteBatch
        /// that need to be made after LoadContent go here.
        /// </summary>
        protected void InitializeAfterLoadContent()
        {
            // Most of these are in NewGame()
            buttons = new Dictionary<GameState, List<Button>>
            {
                { GameState.Menu,
                    new List<Button> {
                        new Button(null, new Vector2(109, 124), 498, 284, 0, Color.White, null, null, null),
                        new Button(null, new Vector2(762, 80), 420, 145, 0, Color.White, null, null, null),
                        new Button(null, new Vector2(776, 387), 417, 136, 0, Color.White, null, null, null),
                        new Button(null, new Vector2(97, 459), 607, 195, 0, Color.White, null, null, null),
                        new Button(null, new Vector2(0, 0), 163, 79, 0, Color.White, null, null, null)
                    }
                },
                { GameState.Paused,
                    new List<Button> {
                        new Button(blankTex, new Vector2(25), 300, 300, 1, Color.DarkOrange, spriteBatch, new TextObject("", Vector2.Zero, mediumFont, Color.GhostWhite, spriteBatch), null)
                    }
                },
                { GameState.GameOver_Win,
                    new List<Button> {
                        new Button(blankTex, new Vector2((Var.TOTAL_WIDTH - 240) / 2, (Var.TOTAL_HEIGHT - 80) / 2), 240, 80, 2, Color.FromNonPremultiplied(136, 0, 21, 255), spriteBatch, new TextObject("Go To Menu", Vector2.Zero, largeFont, Color.FromNonPremultiplied(136, 0, 21, 255), spriteBatch), new ButtonEvent(GoToMenu))
                    }
                },
                { GameState.GameOver_Lose,
                    new List<Button> {
                        new Button(blankTex, new Vector2(250, (Var.TOTAL_HEIGHT - 80) / 2), 240, 80, 2, Color.FromNonPremultiplied(136, 0, 21, 255), spriteBatch, new TextObject("Go To Menu", Vector2.Zero, largeFont, Color.FromNonPremultiplied(136, 0, 21, 255), spriteBatch), new ButtonEvent(GoToMenu)),
                        new Button(blankTex, new Vector2(600, (Var.TOTAL_HEIGHT - 80) / 2), 240, 80, 2, Color.FromNonPremultiplied(136, 0, 21, 255), spriteBatch, new TextObject("Restart", Vector2.Zero, largeFont, Color.FromNonPremultiplied(136, 0, 21, 255), spriteBatch), new ButtonEvent(RestartGame))
                    }
                }
            };
            buttons[GameState.GameOver_Win].ForEach(bttn => bttn.Color = Color.Black);
            buttons[GameState.GameOver_Lose].ForEach(bttn => bttn.Color = Color.Black);
            buttons[GameState.Paused][0].Color = Color.Black;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Var.BLANK_TEX = blankTex = this.Content.Load<Texture2D>("Blank");
            cursorTex = this.Content.Load<Texture2D>("Cursor");
            stripesTex = this.Content.Load<Texture2D>("Stripes");

            #region Menu
            menuBackgrounds = new Dictionary<GameState, Texture2D>
            {
                { GameState.Menu, this.Content.Load<Texture2D>("Menu/MainMenu") },
                { GameState.Instructions, this.Content.Load<Texture2D>("Menu/Instructions") },
                { GameState.Credits, this.Content.Load<Texture2D>("Menu/Credits") },
                { GameState.Game, this.Content.Load<Texture2D>("Menu/gameScreen") },
                { GameState.GameOver_Win, this.Content.Load<Texture2D>("Menu/Win") },
                { GameState.GameOver_Lose, this.Content.Load<Texture2D>("Menu/Lose") }
            };
            #endregion Menu

            #region HUD
            menuBttnTex = this.Content.Load<Texture2D>("HUD/MenuButton");
            infoBoxTex = this.Content.Load<Texture2D>("HUD/InfoBox");
            #endregion HUD

            #region Spritefonts
            largeFont = this.Content.Load<SpriteFont>("Spritefonts/Large");
            mediumFont = this.Content.Load<SpriteFont>("Spritefonts/Medium");
            normalFont = this.Content.Load<SpriteFont>("Spritefonts/Normal");
            smallFont = this.Content.Load<SpriteFont>("Spritefonts/Small");
            #endregion Spritefonts

            #region Sprites
            ant = this.Content.Load<Texture2D>("Sprites/ant_ani");
            spider = this.Content.Load<Texture2D>("Sprites/spider_ani");
            beetle = this.Content.Load<Texture2D>("Sprites/beetle_ani");
            
            cakeTex = this.Content.Load<Texture2D>("Sprites/cake");
            cakepieceTex = this.Content.Load<Texture2D>("Sprites/cakepiece");

            towerTex = this.Content.Load<Texture2D>("Sprites/can");
            bulletTex = this.Content.Load<Texture2D>("Sprites/Bullet");
            acidTex = this.Content.Load<Texture2D>("Sprites/acid_trap");
            slowTex = this.Content.Load<Texture2D>("Sprites/flypaper");
            zapperTex = this.Content.Load<Texture2D>("Sprites/zapper_ani");
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

                    #region Update
                    drawCursor = true;
                    QuickKeys();

                    if (heldItem != null) {
                        heldItem.Center = mousePoint;
                        drawCursor = false;
                    }

                    NextWave(); // If it can begins next wave
                    
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].Update(animationTotalTime, traps);

                        if (enemies[i].IsActive == false)
                        {
                            if (enemies[i].HasCake)
                            {
                                droppedCake.Add(enemies[i].Point);
                            }
                            enemies.RemoveAt(i);  
                            i--;
                        }
                    }

                    towers.ForEach(tower => tower.Fire(enemies));
                    traps.ForEach(trap => traps.Remove(trap.RemoveIfCan()));

                    foreach(Tower tower in towers)
                    {
                        foreach(Enemy enemy in enemies)
                            tower.CheckCollision(enemy);
                    }

                    hud.Update(animationTotalTime);
                    #endregion Update

                    #region Mouse Over

                    #region Menu Display (note: clicking on it is with other click stuff below)
                    if (hud.MenuButton.Focused == false && mouseRect.Intersects(hud.MenuButton.Rectangle))
                    {
                        hud.StartMenuOpening(animationTotalTime);
                    }
                    else if (hud.MenuButton.Focused)
                    {
                        // This rectangle is the rectangle from top of menu bttn to bottom of last menu option.
                        if (mouseRect.Intersects(new Rectangle(hud.MenuButton.X, hud.MenuButton.Y, hud.MenuButton.Width, (hud.MenuButton.ChildButtons.Last().Y + hud.MenuButton.ChildButtons.Last().Height) - hud.MenuButton.Y))){
                            hud.MenuButton.Focused = true;
                            drawCursor = true;
                        }else
                            hud.MenuButton.Focused = false;
                    }
                    #endregion Menu Display

                    #region Selection Bar Name/Price MouseOver
                    foreach (GameObject itm in hud.SelectionBar)
                    {
                        if (itm.Rectangle.Intersects(mouseRect))
                        {
                            hud.CostWindow.IsActive = true;
                            drawCursor = true;
                            string name = "";
                            int cost = 0;
                            if (itm is Tower)
                            {
                                Tower tower = (Tower)itm;
                                name = Var.towerNames[tower.Type];
                                cost = tower.Cost;
                            }
                            else if (itm is Trap)
                            {
                                Trap trap = (Trap)itm;
                                name = Var.trapNames[trap.Type];
                                cost = trap.Cost;
                            }

                            hud.CostWindow.TextObjects[0].Message = name;
                            hud.CostWindow.TextObjects[1].Message = "$" + cost;
                            int width = hud.CostWindow.TextObjects[0].Width + 10;
                            if (hud.CostWindow.TextObjects[1].Width + 10 > width)
                                width = hud.CostWindow.TextObjects[1].Width + 10;

                            hud.CostWindow.Width = width;
                            hud.CostWindow.Center = new Vector2(itm.Center.X, hud.CostWindow.Center.Y);
                            foreach (TextObject to in hud.CostWindow.TextObjects)
                            {
                                to.Center = new Vector2(itm.Center.X, to.Center.Y);
                            }

                            break;
                        }
                        else
                        {
                            hud.CostWindow.IsActive = false;
                        }
                    }
                    #endregion Selection Bar Name/Price MouseOver

                    #endregion Mouse Over

                    #region If Mouse Clicked
                    if (CheckIfClicked(Var.GAME_AREA))
                    {
                        singlePress = false;

                        #region Menu List Buttons
                        ClickGameDropDownMenuItemsIfCan();
                        #endregion Menu List Buttons

                        #region Select Tower / trap from Selection Bar
                        foreach (GameObject itm in hud.SelectionBar)
                        {
                            if (CheckIfClicked(itm.Rectangle))
                            {
                                if (itm is Tower)
                                {
                                    heldItem = NewTower(((Tower)itm).Type);
                                }
                                else if (itm is Trap)
                                {
                                    heldItem = NewTrap(((Trap)itm).Type);
                                }
                            }
                        }
                        #endregion Select Tower / trap from Selection Bar

                        #region Click Something (normally)
                        if (heldItem == null)
                        {
                            foreach (Tower tower in towers)
                            {
                                if (CheckIfClicked(tower.Rectangle))
                                {
                                    selectedItem = tower;
                                    break;
                                }
                            }

                            if (singlePress == false)
                            {
                                foreach (Trap trap in traps)
                                {
                                    if (CheckIfClicked(trap.Rectangle))
                                    {
                                        selectedItem = trap;
                                        if (selectedItem is Trap_Zapper)
                                        {
                                            ((Trap_Zapper)selectedItem).IsClicked = true;
                                        }
                                        break;
                                    }
                                }
                            }

                            if (singlePress == false)
                                selectedItem = null;
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
                                            heldItem = NewTower(((Tower)heldItem).Type);
                                        }
                                    }
                                    else if (tile is Tile_Path)
                                    {
                                        if (heldItem != null && heldItem is Trap && tile.OccupiedBy == null && hud.CanSpendMoney(((Trap)heldItem).Cost))
                                        {
                                            hud.Money -= ((Trap)heldItem).Cost;
                                            ((Trap)heldItem).Place((Tile_Path)tile);
                                            traps.Add(((Trap)heldItem));
                                            heldItem = NewTrap(((Trap)heldItem).Type);
                                        }
                                    }

                                    
                                    break;
                                }
                            }
                        }
                        #endregion Place Something
                    }

                    #region RIGHT Click
                    else if (mouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed)
                    {
                        if (heldItem != null)
                            heldItem = null;
                        else
                            selectedItem = null;
                    }
                    #endregion RIGHT Click

                    #endregion If Mouse Clicked

                    if (hud.Health <= 0)
                        gameState = GameState.GameOver_Lose;

                    break;
                #endregion GameState.Game

                #region Everything else

                #region Paused
                case GameState.Paused:
                    pausedTime += gameTime.ElapsedGameTime;

                    // Allows Menu items to be interactable in pause
                    hud.Update(gameTime);
                    if (hud.MenuButton.Focused == false)
                        hud.StartMenuOpening(gameTime);

                    ClickGameDropDownMenuItemsIfCan();

                    if (SingleKeyPress(Keys.P) || SingleKeyPress(Keys.Space))
                    {
                        gameState = GameState.Game;
                    }
                    else if (CheckIfClicked(buttons[GameState.Paused][0].Rectangle))
                    {
                        debugOn = !debugOn;
                    }
                    break;
                #endregion Paused

                #region Menu
                case GameState.Menu:
                    if (CheckIfClicked(buttons[gameState][0].Rectangle))
                    {
                        NewGame();
                        gameState = GameState.Game;
                    }
                    else if (CheckIfClicked(buttons[gameState][1].Rectangle))
                    {
                        gameState = GameState.Instructions;
                    }
                    else if (CheckIfClicked(buttons[gameState][2].Rectangle))
                    {
                        gameState = GameState.Credits;
                    }
                    else if (CheckIfClicked(buttons[gameState][3].Rectangle) && CheckForSave())
                    {
                        ContinueGame();
                        gameState = GameState.Game;
                    }
                    else if (CheckIfClicked(buttons[gameState][4].Rectangle) || SingleKeyPress(Keys.Escape))
                    {
                        Environment.Exit(0);
                    }
                    break;
                #endregion Menu

                case GameState.Instructions:
                    if (mouseState.RightButton == ButtonState.Pressed)
                        gameState = GameState.Menu;
                    break;
                case GameState.Credits:
                    if (mouseState.RightButton == ButtonState.Pressed)
                        gameState = GameState.Menu;
                    break;

                #region GameOver (Win/Lose)
                case GameState.GameOver_Win:
                case GameState.GameOver_Lose:
                    foreach (Button bttn in buttons[gameState])
                    {
                        bttn.Resize = Vector2.One;

                        if (bttn.Intersects(mouseRect))
                        {
                            bttn.Resize = new Vector2(1.025f);
                            if (CheckIfClicked(bttn.Rectangle))
                                bttn.Click();
                        }
                    }
                    break;
                #endregion GameOver (Win/Lose)

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

                    spriteBatch.Draw(menuBackgrounds[GameState.Game], Var.SCREEN_SIZE, Color.White);
                    map.DrawMap(blankTex, smallFont);

                    cake.Draw();

                    #region Towers
                    Tower mouseOverTower = null;
                    foreach (Tower tower in towers)
                    {
                        tower.Draw();
                        if (mouseRect.Intersects(tower.Rectangle))
                            mouseOverTower = tower;
                    }
                    if (mouseOverTower != null)
                    {
                        DrawCircle(mouseOverTower.Center, mouseOverTower.FireRadius, 64, Color.Red);
                        spriteBatch.DrawString(normalFont, "Price: " + mouseOverTower.Cost + "\nSale Price: " + mouseOverTower.SellCost(), new Vector2(mouseOverTower.Point.X + mouseOverTower.Width + 2, mouseOverTower.Point.Y - 5), Color.Blue);
                    }
                    if (selectedItem is Tower)
                    {
                        if (selectedItem != mouseOverTower)
                            DrawCircle(selectedItem.Center, ((Tower)selectedItem).FireRadius, 64, Color.Red);
                        ImageObject.DrawRectangleOutline(((Tower)selectedItem).OccupiedTile.Rectangle, 3, Color.Blue, blankTex, spriteBatch);
                    }
                    #endregion Towers

                    #region Traps
                    Trap mouseOverTrap = null;
                    foreach (Trap trap in traps)
                    {
                        if (trap is Trap_Zapper)
                            trap.Draw(gameTime);
                        else
                            trap.Draw();

                        if (mouseRect.Intersects(trap.Rectangle))
                            mouseOverTrap = trap;
                    }
                    if (mouseOverTrap != null)
                    {
                        if(mouseOverTrap is Trap_Zapper)
                            spriteBatch.DrawString(normalFont, "Price: " + mouseOverTrap.Cost + "\nSale Price: " + mouseOverTrap.SellCost() + "\nClick to activate", new Vector2(mouseOverTrap.Point.X + mouseOverTrap.Width + 2, mouseOverTrap.Point.Y - 5), Color.Green);
                        else
                            spriteBatch.DrawString(normalFont, "Price: " + mouseOverTrap.Cost + "\nSale Price: " + mouseOverTrap.SellCost(), new Vector2(mouseOverTrap.Point.X + mouseOverTrap.Width + 2, mouseOverTrap.Point.Y - 5), Color.Green);
                    }
                    if (selectedItem is Trap)
                    {
                        ImageObject.DrawRectangleOutline(((Trap)selectedItem).OccupiedTile.Rectangle, 3, Color.Green, blankTex, spriteBatch);
                    }
                    #endregion Traps

                    traps.ForEach(trap => trap.Draw(gameTime));

                    droppedCake.ForEach(dropped => spriteBatch.Draw(cakepieceTex, new Rectangle((int)(dropped.X), (int)(dropped.Y), 15, 15), Color.White));

                    enemies.ForEach(enemy => enemy.Draw(gameTime));

                    if (heldItem != null)
                    {
                        heldItem.Draw();
                        foreach (Tile tile in map.Tiles)
                        {
                            if (mouseRect.Intersects(tile.Rectangle))
                            {
                                if (heldItem is Tower && tile is Tile_Tower && !(tile.OccupiedBy is Cake))
                                {
                                    ImageObject.DrawRectangleOutline(tile.Rectangle, 2, Color.Blue, blankTex, spriteBatch);
                                    DrawCircle(tile.Center, ((Tower)heldItem).FireRadius, 64, Color.Red);
                                }
                                else if (heldItem is Trap && tile is Tile_Path)
                                    ImageObject.DrawRectangleOutline(tile.Rectangle, 2, Color.Green, blankTex, spriteBatch);
                            }
                        }
                    }

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
                    hud.DrawDropDownMenu();
                    spriteBatch.DrawString(largeFont, "Press [P] or [Space] or click \'Pause\'", new Vector2(Var.GAME_AREA.X + (Var.GAME_AREA.Width - largeFont.MeasureString("Press [P] or [Space] or click \'Pause\'").X) / 2, Var.TOTAL_HEIGHT - largeFont.MeasureString("-").Y - 10), Color.YellowGreen);

                    #region Debug
                    if (debugOn)
                    {
                        if (mouseRect.Intersects(buttons[gameState][0].Rectangle))
                        {
                            buttons[gameState][0].Message.Message = "Fine, turn me off.\n\"You can't\nHANDLE\nthe TRUTH!\"";
                            buttons[gameState][0].CenterText();
                            buttons[gameState][0].Draw();
                        }
                        else
                        {
                            buttons[gameState][0].Message.Message = "Cheater.";
                            buttons[gameState][0].CenterText();
                            buttons[gameState][0].Draw();
                        }
                    }
                    else
                    {
                        if (mouseRect.Intersects(buttons[gameState][0].Rectangle))
                        {
                            buttons[gameState][0].Message.Message = "Turn on Debugging.\nYou know you wanna.";
                            buttons[gameState][0].CenterText();
                            buttons[gameState][0].Draw();
                        }
                    }
                    #endregion Debug
                }
                break;

                #endregion GameState.Paused

                case GameState.Menu:
                    spriteBatch.Draw(menuBackgrounds[gameState], Var.SCREEN_SIZE, Color.White);
                    if (CheckForSave() == false)
                        spriteBatch.Draw(blankTex, buttons[gameState][3].Rectangle, Color.Black);
                    break;
                case GameState.Instructions:
                    spriteBatch.Draw(menuBackgrounds[gameState], Var.SCREEN_SIZE, Color.White);
                    spriteBatch.DrawString(mediumFont, "Right click to return", new Vector2(15, Var.TOTAL_HEIGHT - mediumFont.MeasureString("-").Y - 10), Color.DarkGreen);
                    break;
                case GameState.Credits:
                    spriteBatch.Draw(menuBackgrounds[gameState], Var.SCREEN_SIZE, Color.White);
                    spriteBatch.DrawString(mediumFont, "Right click to return", new Vector2(15, Var.TOTAL_HEIGHT - mediumFont.MeasureString("-").Y - 10), Color.DarkGreen);
                    break;
                case GameState.GameOver_Win:
                case GameState.GameOver_Lose:
                    spriteBatch.Draw(menuBackgrounds[gameState], Var.SCREEN_SIZE, Color.White);
                    buttons[gameState].ForEach(bttn => bttn.Draw());
                    buttons[gameState].ForEach(bttn => ImageObject.DrawRectangleOutline(bttn.Resized(), bttn.outLineThickness, Color.FromNonPremultiplied(bttn.outlineColor.R, bttn.outlineColor.G, bttn.outlineColor.B, (byte)(bttn.outlineColor.A * (bttn.Transparency / 100))), Var.BLANK_TEX, spriteBatch));
                    break;
                #endregion Everything else
            }

            if ((drawCursor == true && gameState == GameState.Game) || gameState != GameState.Game)
                spriteBatch.Draw(cursorTex, new Rectangle(mouseRect.X, mouseRect.Y, 25, 25), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion Draw

        #region QuickKeys / Mouse / Keyboard Stuff

        #region Quick Keys
        /// <summary> Called in GameState.Game (NOT GameState.Paused)</summary>
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
                ContinueGame();
            }
            if (SingleKeyPress(Keys.Delete))
            {
                if (selectedItem is Trap)
                {
                    hud.Money += ((Trap)selectedItem).SellCost();
                    ((Trap)selectedItem).OccupiedTile.OccupiedBy = null;
                    traps.Remove((Trap)selectedItem);
                }
                else if (selectedItem is Tower)
                {
                    hud.Money += ((Tower)selectedItem).SellCost();
                    ((Tower)selectedItem).OccupiedTile.OccupiedBy = null;
                    towers.Remove((Tower)selectedItem);
                }
                selectedItem = null;
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

        #endregion QuickKeys / Mouse / Keyboard Stuff

        #region NextLevel-Wave / New-Continue Game

        #region New / Continued Game
        private void SetUpGame()
        {
            gameState = GameState.Game;
            Var.GAME_SPEED = 1;
            difficulty = 1f;
            heldItem = null;
            pausedTime = TimeSpan.Zero;
            animationTotalTime = new GameTime(TimeSpan.Zero, TimeSpan.Zero);
            enemies = new List<Enemy>();
            towers = new List<Tower>();
            traps = new List<Trap>();
            waves = new List<Queue<Enemy>>();
            prevWaves = new List<List<Enemy>>();
            spawnTimer = new Timer(Var.GAME_SPEED);
        }

        public void NewGame()
        {
            SetUpGame();

            SetUpSelection(Var.START_MONEY, Var.MAX_CAKE_HEALTH);

            level = wave = 0;
            map = new Map(level, spriteBatch);
            if (map.Tiles != null)
            { // Note: This check is also in Load()
                foreach (Tile tile in map.Tiles)
                {
                    if (tile.Rectangle.Intersects(cake.Rectangle))
                        tile.OccupiedBy = cake;
                }
            }

            paths = new List<Path>();
            paths.Add(map.FindPath(map.Tiles[1, 13], map.Tiles[23, 17], 1));

            // Gets enemies
            LoadWaves(level, wave);
            droppedCake = new List<Vector2>();
        }

        public void ContinueGame()
        {
            SetUpGame();
            bool fileLoaded = LoadFile();
            if (fileLoaded == false)
            {
                NewGame();
                return;
            }

            // HUD / Cake / Map declared in LoadGame()

            paths = new List<Path>();
            if (map.Tiles != null)
                paths.Add(map.FindPath(map.Tiles[1, 13], map.Tiles[23, 17], 1));

            // Gets enemies
            LoadWaves(level, wave); // level / wave taken from LoadGame();
        }

        public void SetUpSelection(int money, int cakeLeft)
        {
            // This automatically adds any tower/trap to the selection list.
            List<GameObject> selectionList = new List<GameObject>();
            for (int i = 0; i < Enum.GetNames(typeof(Var.TowerType)).Length; i++)
                selectionList.Add(NewTower((Var.TowerType)i));
            for (int i = 0; i < Enum.GetNames(typeof(Var.TrapType)).Length; i++)
                selectionList.Add(NewTrap((Var.TrapType)i));

            cake = new Cake(cakeLeft, Var.GAME_AREA.Left + (Var.TILE_SIZE * 15), Var.GAME_AREA.Top + (Var.TILE_SIZE * 2), Var.TILE_SIZE * 3, Var.TILE_SIZE * 3, spriteBatch, normalFont, cakeTex);
            hud = new HUD(spriteBatch, money, infoBoxTex, mediumFont, cake, selectionList, stripesTex, this);
            hud.MenuButton.Texture = menuBttnTex;

            List<TextObject> texts = new List<TextObject>
            {
                new TextObject("-", Vector2.Zero, normalFont, Color.GhostWhite, spriteBatch),
                new TextObject("-", Vector2.Zero, normalFont, Color.GhostWhite, spriteBatch)
            };
            hud.CostWindow = new Window(0, (int)(hud.SelectionBar[0].Y - texts[0].Height - texts[1].Height - 10), 100, texts[0].Height + texts[1].Height, spriteBatch, blankTex, null, texts);
            hud.CostWindow.TextObjects[0].Y = hud.CostWindow.Y;
            hud.CostWindow.TextObjects[1].Y = hud.CostWindow.Y + texts[0].Height;
            hud.CostWindow.Color = Var.PAUSE_GRAY;
        }
        #endregion New / Continued Game

        #region Next Wave / Level
        public void NextWave()
        {
            spawnTimer.Update(animationTotalTime, Var.GAME_SPEED);

            if (waves.Count > 0 && waves[0].Count > 0 && spawnTimer.Finished)
            {
                enemies.Add(waves[0].Dequeue());
                enemies.Last().Start(animationTotalTime);

                if (waves[0].Count > 0)
                    spawnTimer.Start(animationTotalTime, Var.TIME_BETWEEN_SPAWNS);
                else
                    spawnTimer.Start(animationTotalTime, Var.TIME_BETWEEN_WAVES);
            }
            if (waves.Count > 1 && waves[0].Count == 0 && spawnTimer.Finished)
            {
                waves.RemoveAt(0);
                wave++;
                prevWaves.Add(waves[0].ToList());
            }

            if (waves.Count == 1 && waves[0].Count == 0 && enemies.Count == 0)
            {
                waves.Clear();
                wave = 0;
                NextLevel();
            }
            else if (prevWaves.Count >= 1)
            {
                bool waveAlive = false;
                foreach (Enemy enemy in prevWaves[0])
                {
                    if (enemy.IsActive || enemy.IsSpawning)
                        waveAlive = true;
                }
                if (waveAlive == false)
                {
                    prevWaves.RemoveAt(0);
                    SaveFile();
                }
            }
        }

        public void NextLevel()
        {
            level++;
            wave = 0;
            LoadWaves(level, wave);

            if (waves.Count == 0)
            {
                gameState = GameState.GameOver_Win;
                DeleteSave();
            }
        }
        #endregion Next Wave / Level

        #endregion NextLevel-Wave / New-Continue Game

        #region File Stuff

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
                        paths[Int32.Parse(infoArray[1])]
                        )
                    );
                }
                if (thisWave.Count > 0)
                    waves.Add(thisWave);
                wave++;
                difficulty += Var.WAVES_DIFFICULTY_INCREASE;
            }
            wave = waveNum;
            if (waves.Count > 0)
                prevWaves.Add(waves[0].ToList());
        }
        #endregion Load Waves

        #region Load
        /// <summary> Loads Game info such as money, cake, level, wave, saved towers, and saved traps. also MAP is created here. </summary>
        public bool LoadFile()
        {
            if (CheckForSave())
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

                    SetUpSelection(Int32.Parse(infoArray[0]), Int32.Parse(infoArray[1]));
                    level = Int32.Parse(infoArray[2]);
                    wave = Int32.Parse(infoArray[3]);

                    // Map is declared here as you need the map to place towers and traps, but you need what level to load.
                    map = new Map(level, spriteBatch);
                    if (map.Tiles != null)
                    { // Note: This check is also in NewGame()
                        foreach (Tile tile in map.Tiles)
                        {
                            if (tile.Rectangle.Intersects(cake.Rectangle))
                                tile.OccupiedBy = cake;
                        }
                    }

                    if (map.Tiles != null)
                    {
                        string curType = reader.ReadLine();
                        Tower tempTower; string[] coord;
                        while (curType == "--Towers--")
                        {
                            firstLine = reader.ReadLine();
                            if (firstLine == null || (firstLine[0] == '-' && firstLine[1] == '-'))
                            {
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
                }
                else{
                    reader.Close();
                    reader.Dispose();
                    return false;
                }
                reader.Close();
                reader.Dispose();
                return true;
            }
            return false;
        }
        #endregion Load

        #region Save
        public void SaveFile()
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

            hud.GameSaved();
        }
        #endregion Save

        #region Check If Saved Game / Delete Save
        public bool CheckForSave()
        {
            if (File.Exists("save.sav"))
            {
                StreamReader reader = new StreamReader("save.sav");
                string line;
                if ((line = reader.ReadLine()) != null && line != "")
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

        public void DeleteSave()
        {
            StreamWriter writer = new StreamWriter("Save.sav");
            writer.Write("");
            writer.Close();
            writer.Dispose();
        }
        #endregion Check If Saved Game / Delete Save

        #endregion File Stuff

        #region New Enemy / Tower / Trap

        #region Enemy
        private Enemy NewEnemy(Var.EnemyType type, Path path)
        {
            #region Ant
            if (type == Var.EnemyType.Ant)
            {
                ImageObject image = new ImageObject(
                    ant,
                    0,
                    0,
                    Var.ENEMY_SIZE,
                    Var.ENEMY_SIZE,
                    4,
                    0,
                    0,
                    30,
                    30,
                    0,
                    0,
                    Color.White,
                    100f,
                    ImageObject.RIGHT,
                    Vector2.Zero,
                    SpriteEffects.None,
                    spriteBatch
                );
                image.CenterOrigin();

                return new Enemy(
                    image,
                    (int)(10 * difficulty),
                    (int)(2 * difficulty),
                    1.3f * difficulty,
                    path,
                    blankTex,
                    cakepieceTex,
                    hud
                );
            }
            #endregion Ant

            #region Spider
            if (type == Var.EnemyType.Spider)
            {
                ImageObject image = new ImageObject(
                    spider,
                    0,
                    0,
                    Var.ENEMY_SIZE,
                    Var.ENEMY_SIZE,
                    4,
                    0,
                    0,
                    30,
                    30,
                    0,
                    0,
                    Color.White,
                    100f,
                    ImageObject.RIGHT,
                    Vector2.Zero,
                    SpriteEffects.None,
                    spriteBatch
                );
                image.CenterOrigin();

                return new Enemy(
                    image,
                    (int)(15 * difficulty),
                    (int)(2 * difficulty),
                    1f * difficulty,
                    path,
                    blankTex,
                    cakepieceTex, 
                    hud
                );
            }
            #endregion Spider

            #region Beetle
            if (type == Var.EnemyType.Beetle)
            {
                ImageObject image = new ImageObject(
                    beetle,
                    0,
                    0,
                    Var.ENEMY_SIZE,
                    Var.ENEMY_SIZE,
                    4,
                    0,
                    0,
                    30,
                    30,
                    0,
                    0,
                    Color.White,
                    100f,
                    ImageObject.RIGHT,
                    Vector2.Zero,
                    SpriteEffects.None,
                    spriteBatch
                );
                image.CenterOrigin();

                return new Enemy(
                    image,
                    (int)(23 * difficulty),
                    (int)(2 * difficulty),
                    .7f * difficulty,
                    path,
                    blankTex,
                    cakepieceTex,
                    hud
                );
            }
            #endregion Beetle

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
                return new Tower_Basic(
                    150,
                    health,
                    100,
                    1,
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
            switch (type)
            {
                #region Basic
                case Var.TrapType.Basic:
                    return LoadTrap(type, 5);
                #endregion Basic

                #region Slow
                case Var.TrapType.Slow:
                    return LoadTrap(type, 5);
                #endregion Slow

                #region Zapper
                case Var.TrapType.Zapper:
                    return LoadTrap(type, 300);
                #endregion Zapper
            }

            return null;
        }

        private Trap LoadTrap(Var.TrapType type, int health)
        {
            #region Basic
            if (type == Var.TrapType.Basic)
            {
                ImageObject image = new ImageObject(
                    acidTex,
                    0, 0,
                    Var.TRAP_SIZE, Var.TRAP_SIZE,
                    spriteBatch
                );

                return new Trap_Basic(
                    health,
                    5,
                    50,
                    image,
                    blankTex
                );
            }
            #endregion Basic

            #region Slow
            if (type == Var.TrapType.Slow)
            {
                ImageObject image = new ImageObject(
                    slowTex,
                    0, 0,
                    Var.TRAP_SIZE, Var.TRAP_SIZE,
                    spriteBatch
                );

                return new Trap_Slow(
                    health,
                    6,
                    25,
                    image,
                    slowTex
                );
            }
            #endregion Slow

            #region Fire
            if (type == Var.TrapType.Zapper)
            {
                ImageObject image = new ImageObject(
                    zapperTex,
                    0, 0,
                    Var.TRAP_SIZE, Var.TRAP_SIZE,
                    spriteBatch
                );

                return new Trap_Zapper(
                    health,
                    1,
                    100,
                    image,
                    zapperTex
                );
            }
            #endregion Fire

            return null;
        }
        #endregion Trap

        #endregion New Enemy / Tower / Trap

        #region Button Event Methods
        public void SwitchPauseAndGame(Button bttn)
        {
            if (gameState == GameState.Game)
                gameState = GameState.Paused;
            else
                gameState = GameState.Game;
        }

        public void GoToMenu(Button bttn)
        {
            gameState = GameState.Menu;
        }

        public void RestartGame(Button bttn)
        {
            NewGame();
        }

        #region Game Speed Buttons
        public void GameSpeedOne(Button bttn)
        {
            GameSpeedChange(1, bttn);
        }
        public void GameSpeedTwo(Button bttn)
        {
            GameSpeedChange(2, bttn);
        }
        public void GameSpeedFour(Button bttn)
        {
            GameSpeedChange(4, bttn);
        }

        private void GameSpeedChange(int speed, Button bttn)
        {
            Var.GAME_SPEED = speed;
            hud.MenuButton.ChildButtons[1].ChildButtons.ForEach(button => button.Message.Color = Color.Red);
            bttn.Message.Color = Color.Maroon;
        }
        #endregion Game Speed Buttons

        #endregion Button Event Methods

        #region Other

        #region Dropdown Menu
        public void ClickGameDropDownMenuItemsIfCan()
        {
            if (hud.MenuButton.Focused)
            {
                List<Button> tempList = new List<Button>();
                tempList.AddRange(hud.MenuButton.ChildButtons[1].ChildButtons);
                tempList.AddRange(hud.MenuButton.ChildButtons);

                foreach (Button bttn in tempList)
                {
                    if (CheckIfClicked(bttn.Rectangle))
                        bttn.Click();
                }
            }
        }
        #endregion Dropdown Menu

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
                    int lineWidth = 3;
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

        #endregion Other
    }
}

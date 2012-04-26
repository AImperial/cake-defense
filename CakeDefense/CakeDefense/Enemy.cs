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
    class Enemy:GameObject
    {
        #region Attributes
        private Path path;
        private HUD hud;
        private int currentTile;
        private HealthBar healthBar;
        private GameTime time;
        Timer timer;

        private bool spawning, despawning, dying, hasCake;
        private float slowEffect;

        SpriteBatch sprite;
        Texture2D cakepieceTex;
        private List<Enemy> enemies;
        private List<Vector2> dropped;
        #endregion Attributes

        #region Constructor
        public Enemy(ImageObject imageObject, int health, int damage, float speed, Path path, Texture2D healthTex, Texture2D cakepieceTex, HUD hud)
            : base(imageObject, health, damage, speed)
        {
            healthBar = new HealthBar(healthTex, health, imageObject.SpriteBatch, 2, Var.ENEMY_SHOW_HEALTH_TIME, .8f, Color.Red, Color.OrangeRed);
            sprite = imageObject.SpriteBatch;
            this.cakepieceTex = cakepieceTex;
            IsActive = false;
            this.path = path;
            Image.Center = Center = path.Start.Center;
            currentTile = 0;
            slowEffect = 1;
            Image.Transparency = 0;
            Image.Rotation = ImageObject.RIGHT;
            spawning = true;
            hasCake = false;
            timer = new Timer(Var.GAME_SPEED);
            this.hud = hud;
        }
        #endregion Constructor

        #region Properties
        public Path Path
        {
            get { return path; }
            set { path = value; }
        }

        public int CurrentTile
        {
            get { return currentTile; }
        }

        public bool IsSpawning
        {
            get { return spawning; }
        }

        public bool IsDying
        {
            get { return dying; }
        }

        public bool HasCake
        {
            get { return hasCake; }
            set { hasCake = value; }
        }

        /// <summary> Ranges from 0-1. One is normal speed. </summary>
        public float SlowEffect
        {
            get { return slowEffect; }
            set { slowEffect = value; if (value <= Var.ENEMY_SLOW_CAP) { slowEffect = Var.ENEMY_SLOW_CAP; } if (value >= 1) { slowEffect = 1; } }
        }

        /// <summary> Incorperates speed, slow effect, and game speed. </summary>
        public float Speed_Actual
        {
            get { return Speed * Var.GAME_SPEED * slowEffect; }
        }
        #endregion Properties

        #region Methods

        #region Update
        public void Update(GameTime gameTime, List<Trap> traps)
        {
            time = gameTime;
            timer.Update(gameTime, Var.GAME_SPEED);
            if (dying == false && IsActive)
                healthBar.Update(gameTime, CurrentHealth, Center.X, Y);

            if (IsActive)
            {
                if (spawning == false && despawning == false && IsDying == false)
                {
                    MoveBy(Speed_Actual, traps, enemies); // move the enemy (properly)

                    if (path.GetTile(currentTile) == path.End)
                    {
                        despawning = true;
                        timer.Start(gameTime, Var.DESPAWN_TIME);
                        Despawning();
                    }

                    // check attack stuff/etc here
                }
                else if (despawning)
                {
                    Despawning();
                }
                else if (IsDying)
                {
                    Dying();
                }
                else if (spawning)
                {
                    Spawning();
                }
            }
        }
        #endregion Update

        #region Move
        private void MoveBy(float num, List<Trap> traps, List<Enemy> enemies)
        {
            traps.ForEach(trap => trap.AttackIfCan(this, time));

            #region Check for cake
            if (hasCake == false)
            {
                int cakeCount = 0;
                foreach (Tile tile in path.GetTile(currentTile).Neighbors)
                {
                    if (tile != null && tile.OccupiedBy is Cake)
                    {
                        //loop through enemy objects
                        for (int i = 0; i < enemies.Count; i++)
                        {
                            if (enemies[i].hasCake)
                                cakeCount++;
                        }

                        if((cakeCount + dropped.Count) < Var.MAX_CAKE_HEALTH)
                            hasCake = true;
                        break;
                    }
                }
            }
            #endregion Check for cake

            #region Path
            if (path.InRange(currentTile + 1))
            {
                // Move Left
                if (path.GetNextTile(currentTile).Center.X < Center.X)
                {
                    // Encase Rounding a corner
                    if (Center.X - num < path.GetNextTile(currentTile).Center.X)
                    {
                        currentTile++;
                        // How far off it went over center
                        float distOff = num - (Center.X - path.GetTile(currentTile).Center.X);
                        X -= num - distOff;
                        MoveBy(distOff, traps, enemies);
                    }
                    // Otherwise move normally
                    else
                    {
                        Image.Rotation = ImageObject.LEFT;
                        X -= num;
                        if (Center.X == path.GetNextTile(currentTile).Center.X)
                            currentTile++;
                    }
                }
                // Move Up
                else if (path.GetNextTile(currentTile).Center.Y < Center.Y)
                {
                    // Encase Rounding a corner
                    if (Center.Y - num < path.GetNextTile(currentTile).Center.Y)
                    {
                        currentTile++;
                        // How far off it went over center
                        float distOff = num - (Center.Y - path.GetTile(currentTile).Center.Y);
                        Y -= num - distOff;
                        MoveBy(distOff, traps, enemies);
                    }
                    // Otherwise move normally
                    else
                    {
                        Image.Rotation = ImageObject.UP;
                        Y -= num;
                        if (Center.Y == path.GetNextTile(currentTile).Center.Y)
                            currentTile++;
                    }
                }
                // Move Right
                else if (path.GetNextTile(currentTile).Center.X > Center.X)
                {
                    // Encase Rounding a corner
                    if (Center.X + num > path.GetNextTile(currentTile).Center.X)
                    {
                        currentTile++;
                        // How far off it went over center
                        float distOff = num - (path.GetTile(currentTile).Center.X - Center.X);
                        X += num - distOff;
                        MoveBy(distOff, traps, enemies);
                    }
                    // Otherwise move normally
                    else
                    {
                        Image.Rotation = ImageObject.RIGHT;
                        X += num;
                        if (Center.X == path.GetNextTile(currentTile).Center.X)
                            currentTile++;
                    }
                }
                // Move Down
                else if (path.GetNextTile(currentTile).Center.Y > Center.Y)
                {
                    // Encase Rounding a corner
                    if (Center.Y + num > path.GetNextTile(currentTile).Center.Y)
                    {
                        currentTile++;
                        // How far off it went over center
                        float distOff = num - (path.GetTile(currentTile).Center.Y - Center.Y);
                        Y += num - distOff;
                        MoveBy(distOff, traps, enemies);
                    }
                    // Otherwise move normally
                    else
                    {
                        Image.Rotation = ImageObject.DOWN;
                        Y += num;
                        if (Center.Y == path.GetNextTile(currentTile).Center.Y)
                            currentTile++;
                    }
                }
            }
            #endregion Path
        }
        #endregion Move

        #region Enemies and Cake List
        public void updateEnemiesAndDroppedList(List<Enemy> list, List<Vector2> cake)
        {
            enemies = list;
            dropped = cake;
        }
        #endregion Enemies and Cake List

        #region Let Tower find where you will be
        /// <summary> Does NOT WORK on high enemy / game speeds currently. </summary>
        public float CheckWhereIWillBe(Tower tower)
        {
            int ii = 1;
            Vector2 savedPosition = Point;
            int tileNum = currentTile;
            do {
                if (tower.BulletSpeed * Var.GAME_SPEED * ii > Vector2.Distance(tower.Center, Center))
                {
                    // This should be enough? if not, go back to last tile, and then step-by-step (every 1-2 space) go forawrd, and break right after the above if is true
                    break;
                }
                else
                {
                    MoveBy(Speed_Actual, new List<Trap>(), enemies);
                    ii++;
                }
            } while (ii > 0);
            float angle = (float)Math.Atan2((float)Center.Y - tower.Center.Y, (float)Center.X - tower.Center.X);
            Point = savedPosition;
            currentTile = tileNum;
            return angle;
        }
        #endregion Let Tower find where you will be

        #region Spawning / Despawning Stuff
        public void Start(GameTime gameTime)
        {
            IsActive = true;
            Image.Transparency = 0;
            Image.Rotation = ImageObject.RIGHT;
            spawning = true;
            timer.Start(gameTime, Var.SPAWN_TIME);
        }

        /// <summary> Called in Move. Controls enemy's behavior when spawning. </summary>
        private void Spawning()
        {
            if (timer.Finished == false)
            {
                Image.Transparency = timer.Percent * 100;
                Image.Rotation = (float)(Var.SPAWN_SPINS * (Math.PI * 2) * timer.Percent);//percComplete);
            }
            else
            {
                Image.Transparency = 100;
                Image.Rotation = ImageObject.RIGHT;
                timer.End();
                spawning = false;
                IsActive = true;
            }
        }

        /// <summary> Called in Move. Controls enemy's behavior when despawning. </summary>
        private void Despawning()
        {
            if (timer.Finished == false)
            {
                Image.Transparency = (1 - timer.Percent) * 100;
            }
            else
            {
                Image.Transparency = 100;
                Image.Rotation = ImageObject.RIGHT;
                timer.End();
                despawning = false;
                IsActive = false;
                if (hasCake)
                {
                    hud.EnemyGotCake();
                }
            }
        }

        private void Dying()
        {
            if (timer.Finished == false)
            {
                Image.Transparency = timer.Percent * 100;
                Image.Rotation = (float)(Var.DEATH_SPINS * (Math.PI * 2) * timer.Percent);
                Image.Resize = new Vector2(1 - timer.Percent);
                CenterImage();
            }
            else
            {
                Image.Transparency = 100;
                Image.Rotation = ImageObject.RIGHT;
                timer.End();
                IsActive = false;
            }
        }
        #endregion Spawning / Despawning Stuff

        #region Get Damaged (Hit)
        public void Hit(int hitDmg)
        {
            if (IsActive && IsSpawning == false && IsDying == false)
            {
                CurrentHealth -= hitDmg;
                healthBar.Show(time);

                if (CurrentHealth <= 0)
                {
                    dying = true;
                    despawning = false;
                    healthBar.Hide();
                    hud.EnemyDied(CalculateDeathReward());
                    timer.Start(time, Var.DYING_TIME);
                }
            }
        }

        private int CalculateDeathReward()
        {
            return (int)((StartHealth * Speed) / 2.0f);
        }
        #endregion Get Damaged (Hit)

        #endregion Methods

        #region Draw
        public void Draw(GameTime gameTime)
        {
            if (IsActive)
            {
                base.Draw(gameTime, Var.FRAME_SPEED);
                if (hasCake)
                {
                    sprite.Draw(cakepieceTex, new Rectangle((int)(this.X), (int)(this.Y), 15, 15), Color.White);
                }
                healthBar.Draw();
            }
        }
        #endregion Draw
    }
}

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
        private int currentTile;
        private HealthBar healthBar;
        private GameTime time;
        Timer timer;

        private bool spawning, despawning, dying, hasCake;
        private float transparency, slowEffect;
        #endregion Attributes

        #region Constructor
        public Enemy(ImageObject imageObject, int health, int damage, float speed, Path path, Texture2D healthTex)
            : base(imageObject, health, damage, speed)
        {
            healthBar = new HealthBar(healthTex, health, imageObject.SpriteBatch);
            IsActive = false;
            this.path = path;
            Image.Center = Center = path.Start.Center;
            currentTile = 0;
            slowEffect = 1;
            transparency = 0;
            Image.Rotation = ImageObject.RIGHT;
            spawning = true;
            hasCake = false;
            timer = new Timer(Var.GAME_SPEED);
        }
        #endregion Constructor

        #region Properties
        public Path Path
        {
            get { return path; }
            set { path = value; }
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
        }

        /// <summary> Ranges from 0-1. One is normal speed. </summary>
        public float SlowEffect
        {
            get { return slowEffect; }
            set { slowEffect = value; if (value < 0) { slowEffect = 0; } if (value > 1) { slowEffect = 1; } }
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
            timer.Update(gameTime);
            if (dying == false && IsActive)
                healthBar.Update(gameTime, CurrentHealth, Center.X, Y);

            if (IsActive)
            {
                if (spawning == false && despawning == false && IsDying == false)
                {
                    MoveBy(Speed_Actual, traps); // move the enemy (properly)

                    //if path tile is adjacent to cake tiles, take a slice
                    //if(path.GetTile(currentTile) == path.)
                    //{
                        //play animation
                    //}

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
        private void MoveBy(float num, List<Trap> traps)
        {
            traps.ForEach(trap => trap.AttackIfCan(this));

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
                        MoveBy(distOff, traps);
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
                        MoveBy(distOff, traps);
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
                        MoveBy(distOff, traps);
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
                        MoveBy(distOff, traps);
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
        }
        #endregion Move

        /// <summary> Does NOT WORK on high enemy / game speeds currently. </summary>
        public float CheckWhereIWillBe(Tower tower)
        {
            int ii = 1;
            Vector2 savedPosition = Point;
            int tileNum = currentTile;
            do {
                if (tower.BulletSpeed * ii > Vector2.Distance(tower.Center, Center))
                {
                    //got to do more than this!
                    break;
                }
                else
                {
                    MoveBy(Speed_Actual, new List<Trap>());
                    ii++;
                }
            } while (ii > 0);
            float angle = (float)Math.Atan2((float)Center.Y - tower.Center.Y, (float)Center.X - tower.Center.X);
            Point = savedPosition;
            currentTile = tileNum;
            return angle;
        }

        #region Spawning / Despawning Stuff
        public void Start(GameTime gameTime)
        {
            IsActive = true;
            transparency = 0;
            Image.Rotation = ImageObject.RIGHT;
            spawning = true;
            timer.Start(gameTime, Var.SPAWN_TIME);
        }

        /// <summary> Called in Move. Controls enemy's behavior when spawning. </summary>
        private void Spawning()
        {
            if (timer.Finished == false)
            {
                transparency = timer.Percent;
                Image.Rotation = (float)(Var.SPAWN_SPINS * (Math.PI * 2) * timer.Percent);//percComplete);
            }
            else
            {
                transparency = 100;
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
                transparency = 1 - timer.Percent;
            }
            else
            {
                transparency = 100;
                Image.Rotation = ImageObject.RIGHT;
                timer.End();
                despawning = false;
                IsActive = false;
            }
        }

        private void Dying()
        {
            if (timer.Finished == false)
            {
                transparency = timer.Percent;
                Image.Rotation = (float)(Var.DEATH_SPINS * (Math.PI * 2) * timer.Percent);
                Image.Resize = new Vector2(1 - timer.Percent);
                CenterImage();
            }
            else
            {
                transparency = 100;
                Image.Rotation = ImageObject.RIGHT;
                timer.End();
                IsActive = false;
            }
        }
        #endregion Spawning / Despawning Stuff

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
                    timer.Start(time, Var.DYING_TIME);
                }
            }
        }

        #endregion Methods

        #region Draw
        public void Draw(GameTime gameTime)
        {
            if (IsActive)
            {
                #region Spawning / Despawning / Dying
                if (spawning || despawning || IsDying)
                {
                    Color tempColor = Image.Color;
                    Image.Color = Var.EffectTransparency(transparency, Image.Color);
                    base.Draw(gameTime, Var.FRAME_SPEED);
                    Image.Color = tempColor;

                    if (despawning)
                        healthBar.Draw();
                }
                #endregion Spawning / Despawning / Dying
                else
                {
                    base.Draw(gameTime, Var.FRAME_SPEED);
                    healthBar.Draw();
                }
            }
        }
        #endregion Draw
    }

    #region Health Bar
    class HealthBar
    {
        #region Attributes
        Texture2D texture;
        SpriteBatch spriteBatch;

        Timer timer;
        int originalWidth, heightUpExtra, maxHealth, health;
        Rectangle position;
        #endregion Attributes

        #region Constructor
        public HealthBar(Texture2D texture, int maxHealth, SpriteBatch sprite)
        {
            this.texture = texture;
            spriteBatch = sprite;

            timer = new Timer(Var.GAME_SPEED);
            this.maxHealth = maxHealth;
            heightUpExtra = 2;

            if (maxHealth <= Var.HEALTHBAR_SIZE_MAX.X)
                originalWidth = maxHealth;
            else
                originalWidth = Var.HEALTHBAR_SIZE_MAX.X;

            position = new Rectangle(0, 0, originalWidth, Var.HEALTHBAR_SIZE_MAX.Y);
        }
        #endregion Constructor

        #region Properties
        public int OriginalWidth
        {
            get { return originalWidth; }

            set { originalWidth = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }

            set { texture = value; }
        }
        #endregion Properties

        #region Methods
        public void Show(GameTime gameTime)
        {
            timer.Start(gameTime, Var.SHOW_HEALTH_TIME);
        }

        public void Update(GameTime gameTime, int hp, float centerX, float yVal)
        {
            timer.Update(gameTime);
            health = hp;
            position.X = (int)centerX - (originalWidth / 2);
            position.Y = (int)yVal - position.Height - heightUpExtra;
            position.Width = (int)(originalWidth * ((float)hp / maxHealth));
        }

        public void Hide()
        {
            timer.End();
        }
        #endregion Methods

        #region Draw
        public void Draw()
        {
            if (timer.Finished == false)
            {
                Color colorBar = Color.Red;
                Color colorCaps = Color.OrangeRed;
                int capsWidth = 2;

                if (timer.Percent >= .8)
                {
                    colorBar = Var.EffectTransparency(1 - Timer.GetPercentRelative(.8f, timer.Percent, 1f), colorBar);
                    colorCaps = Var.EffectTransparency(1 - Timer.GetPercentRelative(.8f, timer.Percent, 1f), colorCaps);
                }

                spriteBatch.Draw(texture, position, colorBar);

                if (originalWidth > position.Height)
                {
                    spriteBatch.Draw(texture, new Rectangle(position.X - capsWidth, position.Y - capsWidth, capsWidth, position.Height + (capsWidth * 2)), colorCaps);
                    spriteBatch.Draw(texture, new Rectangle(position.X, position.Y - capsWidth, position.Height, capsWidth), colorCaps);
                    spriteBatch.Draw(texture, new Rectangle(position.X, position.Y + position.Height, position.Height, capsWidth), colorCaps);

                    spriteBatch.Draw(texture, new Rectangle(position.X + originalWidth, position.Y - capsWidth, capsWidth, position.Height + (capsWidth * 2)), colorCaps);
                    spriteBatch.Draw(texture, new Rectangle(position.X + originalWidth - position.Height, position.Y - capsWidth, position.Height, capsWidth), colorCaps);
                    spriteBatch.Draw(texture, new Rectangle(position.X + originalWidth - position.Height, position.Y + position.Height, position.Height, capsWidth), colorCaps);

                }
                else
                {
                    ImageObject.DrawRectangleOutline(new Rectangle(position.X, position.Y, originalWidth, position.Height), capsWidth, colorCaps, texture, spriteBatch);
                }
            }
        }
        #endregion Draw
    }
    #endregion Health Bar
}

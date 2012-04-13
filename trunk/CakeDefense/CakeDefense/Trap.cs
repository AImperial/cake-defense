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
    class Trap:GameObject
    {
        #region Attributes
        protected bool placed;
        protected Tile_Path occupiedTile;
        protected HealthBar healthBar;
        protected int cost;
        protected Var.TrapType type;
        #endregion Attributes

        #region Constructor
        public Trap(int health, int damage, int cost, ImageObject io, Texture2D healthTex)
            :base(io, health, damage, 0)
        {
            type = Var.TrapType.Basic;
            healthBar = new HealthBar(healthTex, StartHealth, io.SpriteBatch, 5, Var.TRAP_SHOW_HEALTH_TIME, .5f, Color.Green, Color.DarkSeaGreen);
            healthBar.OriginalWidth = 50;
            placed = false;
            this.cost = cost;
            Image.Transparency = Var.PLACING_TRANSPARENCY;
        }
        #endregion Constructor

        #region Properties
        public bool Placed
        {
            get { return placed; }
            set { placed = value; }
        }

        public Tile_Path OccupiedTile
        {
            get { return occupiedTile; }
            set { occupiedTile = value; }
        }

        public Var.TrapType Type
        {
            get { return type; }

            set { type = value; }
        }
        #endregion Properties

        #region Methods
        public void Place(Tile_Path tile)
        {
            placed = true;
            occupiedTile = tile;
            occupiedTile.OccupiedBy = this;
            Center = tile.Center;
            Image.Transparency = 100;
        }

        public void AttackIfCan(Enemy enemy, GameTime gameTime)
        {
            if(enemy.Center == Center && IsActive)
            {
                CurrentHealth--;
                enemy.Hit(Damage);
                healthBar.Show(gameTime);
                if (CurrentHealth <= 0)
                    IsActive = false;
            }
        }

        /// <summary> If it should be removed, returns itself; else retruns null </summary>
        public Trap RemoveIfCan()
        {
            if (IsActive == false)
            {
                occupiedTile.OccupiedBy = null;
                return this;
            }
            return null;
        }

        public int Cost
        {
            get { return cost; }
            set { cost = value; }
        }
        #endregion Methods

        #region Draw
        public void Draw(GameTime gameTime)
        {
            if (IsActive)
            {
                healthBar.Update(gameTime, CurrentHealth, Center.X, Y);
                base.Draw();
                healthBar.Draw();
            }
        }
        #endregion Draw
    }
}

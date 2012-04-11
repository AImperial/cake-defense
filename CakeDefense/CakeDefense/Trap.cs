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
        private int cost;
        #endregion Attributes

        #region Constructor
        public Trap(int health, int damage, int cost, ImageObject io)
            :base(io, health, damage, 0)
        {
            placed = false;
            this.cost = cost;
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
        #endregion Properties

        #region Methods
        public void Place(Tile_Path tile)
        {
            placed = true;
            occupiedTile = tile;
            occupiedTile.OccupiedBy = this;
            Center = tile.Center;
        }

        public void AttackIfCan(Enemy enemy)
        {
            if(enemy.Center == Center && IsActive)
            {
                CurrentHealth--;
                enemy.Hit(Damage);
                if (CurrentHealth <= 0)
                    IsActive = false;
            }
        }

        /// <summary> If it should be removed, returns itself; else retruns null </summary>
        public Trap RemoveIfCan()
        {
            if (IsActive == false)
                return this;
            return null;
        }

        public int Cost
        {
            get { return cost; }
            set { cost = value; }
        }
        #endregion Methods

        #region Draw
        public override void Draw()
        {
            if (IsActive)
            {
                #region Placing
                if (placed == false)
                {
                    Color tempColor = Image.Color;
                    Image.Color = Var.PLACING_COLOR;
                    base.Draw();
                    Image.Color = tempColor;
                }
                #endregion Placing

                else
                {
                    base.Draw();
                }
            }
        }
        #endregion Draw
    }
}

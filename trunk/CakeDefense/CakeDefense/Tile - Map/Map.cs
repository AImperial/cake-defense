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
    class Map
    {
        #region Attributes
        private Tile[,] tiles;
        private Point size;
        SpriteBatch sprite;
        #endregion Attributes

        #region Constructor
        public Map(int tilesWide, int tilesHigh, SpriteBatch spriteBatch)
        {
            sprite = spriteBatch;
            size = new Point(tilesWide, tilesHigh);
            //create list for use in loop to change specific tiles
            //32 wide 18 high

            int [,] tileMap = new int [,]{
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0},
                {1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,1,1,1,1,0,0,0,0},
                {0,0,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,1,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,1,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,1,1,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0}
            };

            tiles = new Tile[tileMap.GetUpperBound(1) + 1, tileMap.GetUpperBound(0) + 1];
            for (int i = 0; i <= tileMap.GetUpperBound(1); i++ )
            {
                for (int j = 0; j <= tileMap.GetUpperBound(0); j++)
                {
                    if (tileMap[j, i] == 0)
                        tiles[i, j] = new Tile_Tower(i * Var.TILE_SIZE, j * Var.TILE_SIZE, Var.TILE_SIZE, Var.TILE_SIZE, new Point(i,j));
                    else if (tileMap[j, i] >= 1)
                        tiles[i, j] = new Tile_Path(i * Var.TILE_SIZE, j * Var.TILE_SIZE, Var.TILE_SIZE, Var.TILE_SIZE, new Point(i, j), tileMap[j, i] - 1);
                }
            }
            AssignTileNeighbors();
        }
        #endregion region Constructor

        #region Properties
        public Tile[,] Tiles
        {
            get { return tiles; }
        }
        #endregion Properties

        #region Methods
        public void AssignTileNeighbors()
        {
            // Now for each of the search nodes, we will
            // connect it to each of its neighbours.
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    // An array of all of the possible neighbors this 
                    // node could have. (We will ignore diagonals for now.)
                    Point[] neighbors = new Point[]
                    {
                        new Point (x, y - 1), // The node above the current node
                        new Point (x, y + 1), // The node below the current node.
                        new Point (x - 1, y), // The node left of the current node.
                        new Point (x + 1, y), // The node right of the current node

                    };

                    // We loop through each of the possible neighbors
                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        Point position = neighbors[i];

                        // We need to make sure this neighbour is part of the level.
                        if (position.X < 0 || position.X >= size.X || position.Y < 0 || position.Y >= size.Y)
                        {
                            continue;
                        }

                        // Store a reference to the neighbor.
                        tiles[x, y].Neighbors[i] = tiles[position.X, position.Y];
                    }
                }
            }
        }

        public Path FindPath(Tile startNode, Tile endNode, int pathNum)
        {
            List<Tile> path = new List<Tile>();
            Tile lastTile = startNode;
            Tile currentTile = startNode;
            path.Add(currentTile);
            bool[,] visited = new bool[tiles.GetUpperBound(0) + 1, tiles.GetUpperBound(1) + 1];
            visited[currentTile.TileNum.X, currentTile.TileNum.Y] = true;
            bool movesLeft = true;

            while (currentTile != endNode && movesLeft == true)
            {
                movesLeft = false;
                foreach (Tile neighbor in currentTile.Neighbors)
                {
                    if (neighbor is Tile_Path && visited[neighbor.TileNum.X, neighbor.TileNum.Y] == false && ((Tile_Path)neighbor).Type <= pathNum)
                    {
                        movesLeft = true;
                        visited[neighbor.TileNum.X, neighbor.TileNum.Y] = true;
                        lastTile = currentTile;
                        currentTile = neighbor;
                        path.Add(currentTile);
                        break;
                    }
                }
            }

            return new Path(path);
        }
        #endregion Methods

        #region Draw
        public void DrawMap(Texture2D tex, SpriteFont font)
        {
            foreach (Tile tile in tiles)
            {
                if (tile is Tile_Tower)
                    sprite.Draw(tex, tile.Rectangle, Color.LightGray);
                else if (tile is Tile_Path)
                    sprite.Draw(tex, tile.Rectangle, Color.Gray);
                //sprite.DrawString(font, "x:" + tile.TileNum.X.ToString("00") + "\ny:" + tile.TileNum.Y.ToString("00"), tile.Point, Color.Green);
            }
        }
        #endregion Draw
    }
}

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
        //this can be scaled however is needed. right now its set to 18 x 18 tiles with a border of 2
        //not very hard to then draw a path from this based on the grid
        //actual tiles can be implemented just as easily

        public Map()
        {
        }

        public void DrawMap(SpriteBatch sprite, Texture2D tex)
        {
            // Draw the map
            for (int i = 0; i < 36; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    sprite.Draw(tex, new Rectangle(j * 20, i * 20, 18, 18), Color.Black);
                }
            }
        }
    }
}

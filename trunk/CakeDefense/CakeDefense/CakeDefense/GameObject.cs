using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CakeDefense
{
    class GameObject
    {
        protected int strtHlth; // Max Health
        protected int currHealth; // Current Health
        protected int dmgDn; // Damage that the object does
        protected bool isActive; // If the object is currently active
        protected int speed; // Either Speed of Enemy or Speed of Bullet
        protected Rectangle location; // The current location and size of the object
        protected SpriteBatch sprBtch; // Objects Picture
        protected Color type; // Color shows the type of enemy or tower
        protected Texture pic; // Picture

        

        public GameObject(int sh, int dd, int sp, int x, int y, int w, int h, SpriteBatch spB, Color t, Texture p)
        {
            strtHlth = sh;
            currHealth = sh;
            dmgDn = dd;
            isActive = true;
            speed = sp;
            location = new Rectangle(x, y, w, h);
            sprBtch = spB;
            type = t;
            pic = p;
        }
    }
}

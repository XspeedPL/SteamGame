using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SteamGame
{
    public static class Camera
    {
        static Vector2 position = new Vector2();
        static float speed = 4.0F;

        public static float Speed
        {
            get { return speed; }
            set
            {
                speed = MathHelper.Clamp(value, 0.5f, 20f);
            }
        }

        public static Vector2 Position
        {
            get { return position; }

            set
            {
                position.X = MathHelper.Clamp(value.X, 0, Main.map.Width * Main.defSize - Main.ScreenWidth);
                position.Y = MathHelper.Clamp(value.Y, 0, Main.map.Height * Main.defSize - Main.ScreenHeight + 48);
            }
        }
    }
}
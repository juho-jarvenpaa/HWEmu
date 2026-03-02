using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HWEmu
{
    public class Psu
    {
        public Rectangle Bounds { get; set; }

        public Psu(Rectangle rectangle)
        {
            Bounds = rectangle;
        }

        public static void DrawPSU(Psu psu)
        {
            Raylib.DrawRectangle((int)psu.Bounds.X, (int)psu.Bounds.Y, (int)psu.Bounds.Width, (int)psu.Bounds.Height, Color.Blue);
            Raylib.DrawText("PSU", (int)psu.Bounds.X + 20, (int)psu.Bounds.Y + 30, 30, Color.Yellow);
        }
    }
}
